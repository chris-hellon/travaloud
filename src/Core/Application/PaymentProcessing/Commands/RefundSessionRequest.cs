using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Stripe;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class RefundSessionRequest : IRequest<bool>
{
    public string? StripeChargeId { get; set; }
    public string? PaymentIntendId { get; set; }
    public decimal Total { get; set; }

    public RefundSessionRequest(string? stripeChargeId, string? paymentIntendId, decimal total)
    {
        StripeChargeId = stripeChargeId;
        PaymentIntendId = paymentIntendId;
        Total = total;
    }
}

internal class RefundSessionRequestHandler : IRequestHandler<RefundSessionRequest, bool>
{
    private readonly IStripeClient _stripeClient;

    public RefundSessionRequestHandler(IOptions<StripeSettings> stripeSettings,
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
            var tenantSettings =
                multiTenantStripeSettings.Tenants.FirstOrDefault(x => x.TenantIdentifier == tenantIdentifier);

            if (tenantSettings == null)
            {
                throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");
            }

            if (tenantSettings.Environments == null)
                throw new ArgumentException($"No settings found for tenant '{tenantIdentifier}'.");

            foreach (var environment in tenantSettings.Environments.Where(environment =>
                         environment.Environment == hostingEnvironment.EnvironmentName))
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

    public async Task<bool> Handle(RefundSessionRequest request, CancellationToken cancellationToken)
    {
        var service = new RefundService(_stripeClient);
        var options = new RefundCreateOptions {Charge = request.StripeChargeId, Amount = request.Total.ConvertToCents()};
        var result = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return result != null;
    }
}