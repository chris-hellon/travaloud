using Microsoft.Extensions.Options;
using Stripe;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.PaymentProcessing.Extensions;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreatePaymentIntentRequest : IRequest<string>
{
    public BasketModel? Basket { get; set; }
}

internal class CreatePaymentIntentRequestHandler : IRequestHandler<CreatePaymentIntentRequest, string>
{
    private readonly IStripeClient _stripeClient;

    public CreatePaymentIntentRequestHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }


    public async Task<string> Handle(CreatePaymentIntentRequest request, CancellationToken cancellationToken)
    {
        if (request.Basket == null) return string.Empty;
        
        var propertiesLineItemsParsed =  request.Basket?.Items.Where(x => x is {PropertyId: not null, Rooms: not null}).GetSessionLineItemOptions(true);
        var toursLineItemsParsed = request.Basket?.Items.Where(x => x is {TourId: not null, TourDates: not null}).GetSessionLineItemOptions(false);

        var propertiesLineItems = propertiesLineItemsParsed?.Item2;
        var toursLineItems = toursLineItemsParsed?.Item2;
        
        var propertiesLabel = propertiesLineItemsParsed?.Item1;
        var toursLabel = propertiesLineItemsParsed?.Item1;
        
        var lineItems = propertiesLineItems!.Union(toursLineItems!).ToList();

        var description = string.Join(", ", propertiesLabel, toursLabel);
        description = description.Trim().TrimEnd(',');
        //description += $" - Booking: {request.InvoiceId.ToString()}";
        
        var paymentIntentService = new PaymentIntentService(_stripeClient);
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = request.Basket.Total.ConvertToCents(),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
            },
            Description = description
        }, cancellationToken: cancellationToken);

        return paymentIntent.ClientSecret;
    }
}