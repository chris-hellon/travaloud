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
    
    public RefundSessionRequestHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }
    
    public async Task<bool> Handle(RefundSessionRequest request, CancellationToken cancellationToken)
    {
        var service = new RefundService(_stripeClient);
        var options = new RefundCreateOptions { Charge = request.StripeChargeId};
        var result = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return result != null;
    }
}