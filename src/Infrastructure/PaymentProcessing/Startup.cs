using System.Buffers;
using System.Text;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.PaymentProcessing;

namespace Travaloud.Infrastructure.PaymentProcessing;

public static class Startup
{
    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration config)=>
        services.Configure<StripeSettings>(config.GetSection(nameof(StripeSettings)));
    
    public static IServiceCollection AddMultiTenantStripe(this IServiceCollection services, IConfiguration config)=>
        services.Configure<MultiTenantStripeSettings>(config.GetSection(nameof(MultiTenantStripeSettings)));

    public static IEndpointConventionBuilder MapStripeWebhookEndpoint(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var webhooksGroup = endpoints.MapGroup("/webhooks");
        
        webhooksGroup.MapPost("stripe-payment", async (
            HttpContext context,
            [FromServices] Serilog.ILogger logger,
            [FromServices] IBookingsService bookingsService,
            [FromServices] IOptions<MultiTenantStripeSettings> multiTenantStripeOptions,
            [FromServices] IMultiTenantContextAccessor multiTenantContextAccessor,
            [FromServices] IHostingEnvironment hostingEnvironment) =>
        {
            var multiTenantStripeSettings = multiTenantStripeOptions.Value;
            var tenantIdentifier = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier;
            var tenantSettings = multiTenantStripeSettings.Tenants.FirstOrDefault(x => x.TenantIdentifier == tenantIdentifier);

            var webhookSecret = string.Empty;
            
            foreach (var environment in tenantSettings.Environments.Where(environment => environment.Environment == hostingEnvironment.EnvironmentName))
            {
                if (environment.StripeSettings == null)
                    throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            
                if (environment.StripeSettings != null)
                {
                    webhookSecret = environment.StripeSettings.WebhookSecret;
                }
            
                break;
            }

            logger.Information("Stripe Webhook Triggered");
            
            var json = await new StreamReader(context.Request.Body).ReadToEndAsync();

            try
            {
                logger.Information("Webhook requeest body :{requestBody}", json);
                
                var stripeEvent = EventUtility.ConstructEvent(json,
                    context.Request.Headers["Stripe-Signature"], webhookSecret, throwOnApiVersionMismatch: false);
                
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    logger.Information("Stripe Checkout Session Completed");

                    if (stripeEvent.Data.Object is not Session checkoutSession || string.IsNullOrEmpty(checkoutSession.ClientReferenceId)) return Results.Ok();
                    
                    var bookingId = checkoutSession.ClientReferenceId;

                    if (string.IsNullOrEmpty(bookingId) || !DefaultIdType.TryParse(bookingId, out var bookingIdParsed))
                        return Results.Ok();
                    
                    await bookingsService.FlagBookingAsPaidAsync(bookingIdParsed, new FlagBookingAsPaidRequest()
                    {
                        Id = bookingIdParsed
                    });
                    
                    logger.Information("Booking flagged as paid from webhook: {0}", bookingId);
                }
                else
                {
                    logger.Information("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Results.Ok();
            }
            catch (StripeException ex)
            {
                logger.Error(ex, ex.Message);
                return Results.BadRequest();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                return Results.StatusCode(500);
            }
        }).AllowAnonymous();

        return webhooksGroup;
    }
}