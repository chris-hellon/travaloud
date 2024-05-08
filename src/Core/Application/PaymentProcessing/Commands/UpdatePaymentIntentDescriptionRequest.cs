using Microsoft.Extensions.Options;
using Stripe;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class UpdatePaymentIntentDescriptionRequest : IRequest
{
    public PaymentIntent PaymentIntent { get; set; }
    public int InvoiceId { get; set; }

    public UpdatePaymentIntentDescriptionRequest(
        PaymentIntent paymentIntent, 
        int invoiceId)
    {
        PaymentIntent = paymentIntent;
        InvoiceId = invoiceId;
    }
}

internal class UpdatePaymentIntentDescriptionRequestHandler : IRequestHandler<UpdatePaymentIntentDescriptionRequest>
{
    private readonly IStripeClient _stripeClient;
    private readonly IStripeService _stripeService;

    public UpdatePaymentIntentDescriptionRequestHandler(IOptions<StripeSettings> stripeSettings, IStripeService stripeService)
    {
        _stripeService = stripeService;
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }
    
    public async Task Handle(UpdatePaymentIntentDescriptionRequest request, CancellationToken cancellationToken)
    {
        var options = new PaymentIntentUpdateOptions()
        {
            Description = $"{request.PaymentIntent.Description} - Booking: {request.InvoiceId}"
        };

        var service = new PaymentIntentService(_stripeClient);
        await service.UpdateAsync(request.PaymentIntent.Id, options, cancellationToken: cancellationToken);
    }
}