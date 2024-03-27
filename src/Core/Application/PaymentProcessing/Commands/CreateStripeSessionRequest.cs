using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Travaloud.Application.Basket.Dto;

namespace Travaloud.Application.PaymentProcessing.Commands;

public class CreateStripeSessionRequest : IRequest<Session>
{
    [Required]
    public DefaultIdType? BookingId { get; set; }
    
    [Required]
    public int? InvoiceId { get; set; }
    
    [Required]
    public string? SuccessUrl { get; set; }
    
    [Required]
    public string? CancelUrl { get; set; }
    
    [Required]
    public BasketModel? Basket { get; set; }
    
    public string? CustomerEmail { get; set; }

    public CreateStripeSessionRequest(DefaultIdType? bookingId, int? invoiceId, string? successUrl, string? cancelUrl, BasketModel basket, string? customerEmail)
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
    
    public CreateStripeSessionRequestHandler(IOptions<StripeSettings> stripeSettings)
    {
        _stripeClient = new StripeClient(stripeSettings.Value.ApiSecretKey);
    }
    
    public async Task<Session> Handle(CreateStripeSessionRequest request, CancellationToken cancellationToken)
    {
        var propertiesLineItems = request.Basket?.Items.Where(x => x.PropertyId.HasValue);
        var toursLineItems = request.Basket?.Items.Where(x => x.TourId.HasValue);

        var propertiesLineItemsModels = propertiesLineItems as BasketItemModel[] ?? propertiesLineItems?.ToArray();
        var toursLineItemsModels = toursLineItems as BasketItemModel[] ?? toursLineItems?.ToArray();
        
        var propertiesLineItemsParsed = propertiesLineItemsModels?.Select(x => new SessionLineItemOptions
        {
            Quantity = 1,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = ConvertToCents(x.Total),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.PropertyName,
                    Images = [x.PropertyImageUrl],
                    Description = $"{x.Rooms.Count} room{(x.Rooms.Count > 1 ? "s" : "")} at {x.PropertyName} from {x.CheckInDateParsed.Value.ToShortDateString()} - {x.CheckOutDateParsed.Value.ToShortDateString()}."
                }
            }
        }) ?? Array.Empty<SessionLineItemOptions>();


        var toursLineItemsParsed = toursLineItemsModels?.Select(x => new SessionLineItemOptions
        {
            Quantity = 1,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = ConvertToCents(x.Total),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.Tour?.Name,
                    Description = $"{x.TourDates.Count} date at {x.Tour?.Name} on {string.Join(", ", x.TourDates.Select(td => td.TourDate.StartDate.ToShortDateString()))}"
                }
            }
        }) ?? Array.Empty<SessionLineItemOptions>();

        var lineItems = propertiesLineItemsParsed.Union(toursLineItemsParsed).ToList();
        
        var customerSearchOptions = new CustomerSearchOptions
        {
            Query = $"email:'{request.CustomerEmail}'",
        };
        
        var customerService = new CustomerService(_stripeClient);
        var matchedCustomers = await customerService.SearchAsync(customerSearchOptions, cancellationToken: cancellationToken);

        string? customerId = null;
        
        if (matchedCustomers != null && matchedCustomers.Data.Count != 0)
        {
            customerId = matchedCustomers.Data.First().Id;
        }

        var propertiesLabel = propertiesLineItemsModels != null
            ? string.Join(", ", propertiesLineItemsModels.Select(x => x.PropertyName))
            : "";
        var toursLabel = toursLineItemsModels != null
            ? string.Join(", ", toursLineItemsModels.Select(x => x.Tour.Name))
            : "";
        
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
                    { "BookingId", request.BookingId.ToString() },
                    { "InvoiceId", request.InvoiceId.Value.ToString()},
                    { "Properties", propertiesLabel },
                    { "Tours", toursLabel }
                },
                Description = description
            }
        };

        if (!string.IsNullOrEmpty(customerId))
            options.Customer = customerId;
        else
        {
            options.CustomerEmail = request.CustomerEmail;
            options.CustomerCreation = "if_required";
        }
        
        var service = new SessionService(_stripeClient);

        return await service.CreateAsync(options, cancellationToken: cancellationToken);
    }

    private static int ConvertToCents(decimal dollars)
    {
        return (int)(dollars * 100);
    }
}