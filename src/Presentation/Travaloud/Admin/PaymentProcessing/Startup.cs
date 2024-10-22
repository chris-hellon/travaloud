using BlazorTemplater;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Mailing;
using Travaloud.Application.PaymentProcessing;

namespace Travaloud.Admin.PaymentProcessing;

public static class Startup
{
    public static IEndpointConventionBuilder MapStripeWebhookEndpoint(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var webhooksGroup = endpoints.MapGroup("/webhooks");

        webhooksGroup.MapPost("stripe-payment", async (
            HttpContext context,
            [FromServices] Serilog.ILogger logger,
            [FromServices] IBookingsService bookingsService,
            [FromServices] IMailService mailService,
            [FromServices] IUserService userService,
            [FromServices] IOptions<MultiTenantStripeSettings> multiTenantStripeOptions,
            [FromServices] IMultiTenantContextAccessor multiTenantContextAccessor,
            [FromServices] IWebHostEnvironment hostingEnvironment) =>
        {
            var multiTenantStripeSettings = multiTenantStripeOptions.Value;
            var tenantIdentifier = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier;
            var tenantSettings =
                multiTenantStripeSettings.Tenants.FirstOrDefault(x => x.TenantIdentifier == tenantIdentifier);

            var webhookSecret = string.Empty;

            foreach (var environment in tenantSettings.Environments.Where(environment =>
                         environment.Environment == hostingEnvironment.EnvironmentName))
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
                    logger.Information("Stripe Checkout Session Completed, now Processing booking and sending emails");

                    if (stripeEvent.Data.Object is not Session checkoutSession ||
                        string.IsNullOrEmpty(checkoutSession.ClientReferenceId)) return Results.BadRequest();

                    var bookingId = checkoutSession.ClientReferenceId;

                    if (string.IsNullOrEmpty(bookingId) || !DefaultIdType.TryParse(bookingId, out var bookingIdParsed))
                        return Results.BadRequest();

                    var booking = await bookingsService.GetAsync(bookingIdParsed) ??
                                  throw new StripeException($"No booking found with Id {bookingIdParsed}");

                    if (!booking.IsPaid)
                    {
                        await bookingsService.FlagBookingAsPaidAsync(bookingIdParsed, new FlagBookingAsPaidRequest()
                        {
                            Id = bookingIdParsed
                        });
                        
                        logger.Information("Booking flagged as paid from webhook: {0}", bookingId);
                    }
                    
                    if (!booking.ConfirmationEmailSent.HasValue || !booking.ConfirmationEmailSent.Value)
                    {
                        var emailModel = new BookingConfirmationTemplateModel(
                            tenantSettings.TenantName,
                            tenantSettings.PrimaryBackgroundColor,
                            tenantSettings.SecondaryBackgroundColor,
                            tenantSettings.HeaderBackgroundColor,
                            tenantSettings.TextColor,
                            tenantSettings.LogoImageUrl,
                            booking,
                            booking.InvoiceId,
                            $"{tenantSettings.TenantUrl}/contact",
                            booking.GuestEmail
                        );

                        var emailHtml = new ComponentRenderer<EmailTemplates.BookingConfirmation>()
                            .Set(x => x.Model, emailModel)
                            .Render();

                        var mailRequest = new MailRequest(
                            to: [booking.GuestEmail!],
                            subject: $"{tenantSettings.TenantName} Order Confirmation",
                            body: emailHtml);

                        await mailService.SendAsync(mailRequest);
                        await bookingsService.FlagBookingConfirmationEmailAsync(booking.Id,
                            new FlagBookingConfirmationEmailRequest(booking.Id));
                        
                        logger.Information("Confirmation email sent for BookingId {BookingId}", bookingId);

                        if (booking.Items != null)
                        {
                            var bookedTours = booking.Items.Where(x => x.TourId.HasValue);

                            var basketItemModels = bookedTours as BookingItemDetailsDto[] ?? bookedTours.ToArray();
                            if (basketItemModels.Length != 0)
                            {
                                var distinctSupplierIds = basketItemModels.Where(x => x.Tour != null)
                                    .Select(x => x.Tour.SupplierId.ToString()).Distinct().ToList();
                                var suppliers =
                                    await userService.SearchAsync(distinctSupplierIds, CancellationToken.None);

                                foreach (var supplierId in distinctSupplierIds)
                                {
                                    if (string.IsNullOrEmpty(supplierId))
                                        continue;

                                    var supplier = suppliers.FirstOrDefault(x => x.Id == Guid.Parse(supplierId));

                                    if (supplier == null || string.IsNullOrEmpty(supplier.Email))
                                        continue;

                                    // Send confirmation email
                                    var supplierEmailModel = new SupplierBookingConfirmationTemplateModel(
                                        tenantSettings.TenantName,
                                        tenantSettings.PrimaryBackgroundColor,
                                        tenantSettings.SecondaryBackgroundColor,
                                        tenantSettings.HeaderBackgroundColor,
                                        tenantSettings.TextColor,
                                        tenantSettings.LogoImageUrl,
                                        booking,
                                        booking.InvoiceId,
                                        $"{tenantSettings.TenantUrl}/contact",
                                        booking.GuestEmail
                                    )
                                    {
                                        SupplierId = supplier.Id
                                    };

                                    var supplierEmailHtml =
                                        new ComponentRenderer<EmailTemplates.SupplierBookingConfirmation>()
                                            .Set(x => x.Model, supplierEmailModel)
                                            .Render();

                                    var supplierMailRequest = new MailRequest(
                                        to: [supplier.Email!],
                                        subject: $"{tenantSettings.TenantName} Booking Confirmation",
                                        body: supplierEmailHtml);

                                    await mailService.SendAsync(supplierMailRequest);

                                    logger.Information(
                                        "Supplier email sent for BookingId {BookingId} & Supplier {SupplierName}",
                                        bookingId, supplier.Email);
                                }
                            }
                        }
                    }
                    
                    logger.Information("Stripe Webhook Processed for Booking {BookingId}", bookingId);
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