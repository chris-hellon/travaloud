using System.ComponentModel.DataAnnotations;
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
    
    public GetStripePaymentStatusHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
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