using System.ComponentModel.DataAnnotations;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Travaloud.Application.PaymentProcessing.Queries;

public class GetStripePaymentStatusRequest : IRequest<Session?>
{
    [Required]
    public string? StripeSessionId { get; set; }

    public GetStripePaymentStatusRequest(string? stripeSessionId)
    {
        StripeSessionId = stripeSessionId;
    }
}

internal class GetStripePaymentStatusHandler : IRequestHandler<GetStripePaymentStatusRequest, Session?>
{
    private readonly IStripeClient _stripeClient;
    
    public GetStripePaymentStatusHandler(
        IOptions<StripeSettings> stripeSettings,
        IHostingEnvironment hostingEnvironment, 
        IMultiTenantContextAccessor multiTenantContextAccessor,
        IOptions<MultiTenantStripeSettings> multiTenantStripeOptions)
    {
        if (!string.IsNullOrEmpty(stripeSettings.Value.ApiSecretKey))
            _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
        else
        {
            var multiTenantStripeSettings = multiTenantStripeOptions.Value;
            var tenantIdentifier = multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Identifier;
            var tenantSettings = multiTenantStripeSettings.Tenants.FirstOrDefault(x => x.TenantIdentifier == tenantIdentifier);

            if (tenantSettings == null)
            {
                throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            }
        
            if (tenantSettings.Environments == null) 
                throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            
            foreach (var environment in tenantSettings.Environments.Where(environment => environment.Environment == hostingEnvironment.EnvironmentName))
            {
                if (environment.StripeSettings == null)
                    throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            
                if (environment.StripeSettings != null)
                {
                    _stripeClient = new StripeClient(environment.StripeSettings.ApiSecretKey);
                }
            
                break;
            }
        }
    }
    
    public async Task<Session?> Handle(GetStripePaymentStatusRequest request, CancellationToken cancellationToken)
    {
        var service = new SessionService(_stripeClient);
        var paymentIntentService = new PaymentIntentService(_stripeClient);
        
        var stripeSession = await service.GetAsync(request.StripeSessionId, cancellationToken: cancellationToken);

        if (stripeSession == null) return stripeSession;
        
        var paymentIntent =
            await paymentIntentService.GetAsync(stripeSession.PaymentIntentId,
                cancellationToken: cancellationToken);

        stripeSession.PaymentIntent = paymentIntent;

        return stripeSession;
    }
}