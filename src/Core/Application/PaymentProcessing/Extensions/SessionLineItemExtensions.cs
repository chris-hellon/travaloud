using Stripe.Checkout;
using Travaloud.Application.Basket.Dto;

namespace Travaloud.Application.PaymentProcessing.Extensions;

public static class SessionLineItemExtensions
{
    public static Tuple<string, IEnumerable<SessionLineItemOptions>> GetSessionLineItemOptions(this IEnumerable<BasketItemModel> basketItems, bool isProperty)
    {
        var basketLineItemsModels = basketItems as BasketItemModel[] ?? basketItems?.ToArray();
        
        if (isProperty)
        {
            return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
                ? string.Join(", ", basketLineItemsModels.Select(x => x.PropertyName))
                : "",  basketLineItemsModels?.Select(x => new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = x.Total.ConvertToCents(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = x.PropertyName,
                        Images = [x.PropertyImageUrl],
                        Description =
                            $"{x.Rooms!.Count} room{(x.Rooms.Count > 1 ? "s" : "")} at {x.PropertyName} from {x.CheckInDateParsed?.ToShortDateString()} - {x.CheckOutDateParsed?.ToShortDateString()}."
                    }
                }
            }) ?? Array.Empty<SessionLineItemOptions>());
        }

        return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
            ? string.Join(", ", basketLineItemsModels.Select(x => x.TourName))
            : "", basketLineItemsModels?.Select(x => new SessionLineItemOptions
        {
            Quantity = 1,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = x.Total.ConvertToCents(),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.TourName,
                    Images = [x.TourImageUrl],
                    Description =
                        $"{x.TourDates!.Count} date at {x.TourName} on {string.Join(", ", x.TourDates.Select(td => td.StartDate.ToShortDateString()))}"
                }
            }
        }) ?? Array.Empty<SessionLineItemOptions>());
    }
}