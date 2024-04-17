using Stripe.Checkout;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

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
                        $"{x.GuestCount} guest{(x.GuestCount > 1 ? "s" : "")} at {x.TourName} on {string.Join(", ", x.TourDates.Select(td => td.StartDate.ToLongDateString()))}"
                }
            }
        }) ?? Array.Empty<SessionLineItemOptions>());
    }
    
    public static Tuple<string, IEnumerable<SessionLineItemOptions>> GetSessionLineItemOptions(this IEnumerable<BookingItemDetailsDto> bookingItems, bool isProperty)
    {
        var basketLineItemsModels = bookingItems as BookingItemDetailsDto[] ?? bookingItems?.ToArray();
        
        if (isProperty)
        {
            return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
                ? string.Join(", ", basketLineItemsModels.Select(x => x.Property.Name))
                : "",  basketLineItemsModels?.Select(x => new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = x.Amount.ConvertToCents(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = x.Property.Name,
                        Images = [x.Property.ImagePath],
                        Description =
                            $"{x.Rooms!.Count} room{(x.Rooms.Count > 1 ? "s" : "")} at {x.Property.Name} from {x.StartDate.ToShortDateString()} - {x.EndDate.ToShortDateString()}."
                    }
                }
            }) ?? Array.Empty<SessionLineItemOptions>());
        }

        return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
            ? string.Join(", ", basketLineItemsModels.Select(x => x.Tour.Name))
            : "", basketLineItemsModels?.Select(x => new SessionLineItemOptions
        {
            Quantity = 1,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = x.Amount.ConvertToCents(),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.Tour.Name,
                    Images = [x.Tour.ImagePath],
                    Description =
                        $"{x.Guests?.Count} guest{(x.Guests?.Count > 1 ? "s" : "")} at {x.Tour.Name} on {string.Join(", ", x.StartDate.ToLongDateString())}"
                }
            }
        }) ?? Array.Empty<SessionLineItemOptions>());
    }
}