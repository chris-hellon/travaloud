using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.PaymentProcessing.Extensions;
using Travaloud.Application.PaymentProcessing.Queries;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreateStripeSessionRequest : IRequest<Session>
{
    [Required] public DefaultIdType? BookingId { get; }

    [Required] public int? InvoiceId { get; }

    [Required] public string? SuccessUrl { get; }

    [Required] public string? CancelUrl { get; }

    [Required] public BasketModel? Basket { get; }

    [Required] public string? CustomerEmail { get; }

    public CreateStripeSessionRequest(
        DefaultIdType? bookingId, 
        int? invoiceId, 
        string? successUrl, 
        string? cancelUrl,
        BasketModel basket, 
        string? customerEmail)
    {
        BookingId = bookingId;
        InvoiceId = invoiceId;
        SuccessUrl = successUrl;
        CancelUrl = cancelUrl;
        Basket = basket;
        CustomerEmail = customerEmail;
    }
}

internal class CreateStripeSessionRequestHandler : IRequestHandler<CreateStripeSessionRequest, Session>
{
    private readonly IStripeClient _stripeClient;
    private readonly IStripeService _stripeService;

    public CreateStripeSessionRequestHandler(IOptions<StripeSettings> stripeSettings, IStripeService stripeService)
    {
        _stripeService = stripeService;
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }

    public async Task<Session> Handle(CreateStripeSessionRequest request, CancellationToken cancellationToken)
    {
        var propertiesLineItemsParsed =  request.Basket?.Items.Where(x => x is {PropertyId: not null, Rooms: not null}).GetSessionLineItemOptions(true);
        var toursLineItemsParsed = request.Basket?.Items.Where(x => x is {TourId: not null, TourDates: not null}).GetSessionLineItemOptions(false);

        var propertiesLineItems = propertiesLineItemsParsed?.Item2;
        var toursLineItems = toursLineItemsParsed?.Item2;
        
        var propertiesLabel = propertiesLineItemsParsed?.Item1;
        var toursLabel = propertiesLineItemsParsed?.Item1;
        
        var lineItems = propertiesLineItems!.Union(toursLineItems!).ToList();

        var description = string.Join(", ", propertiesLabel, toursLabel);
        description = description.Trim().TrimEnd(',');
        description += $" - Booking: {request.InvoiceId.ToString()}";

        var options = new SessionCreateOptions
        {
            ClientReferenceId = request.BookingId.ToString(),
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            PaymentIntentData = new SessionPaymentIntentDataOptions()
            {
                ReceiptEmail = request.CustomerEmail,
                Metadata = new Dictionary<string, string>
                {
                    {"BookingId", request.BookingId.ToString()!},
                    {"InvoiceId", request.InvoiceId!.Value.ToString()},
                    {"Properties", propertiesLabel ?? string.Empty},
                    {"Tours", toursLabel ?? string.Empty}
                },
                Description = description
            }
        };
        
        var existingCustomer = await _stripeService.SearchStripeCustomer(new SearchStripeCustomerRequest(request.CustomerEmail!));
            
        string? customerId = null;

        if (existingCustomer != null)
        {
            customerId = existingCustomer.Id;
        }

        if (!string.IsNullOrEmpty(customerId))
            options.Customer = customerId;
        else
        {
            options.CustomerEmail = request.CustomerEmail;
            options.CustomerCreation = "always";
        }

        var service = new SessionService(_stripeClient);

        return await service.CreateAsync(options, cancellationToken: cancellationToken);
    }
}