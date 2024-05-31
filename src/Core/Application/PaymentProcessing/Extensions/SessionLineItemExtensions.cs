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
                        Images = x.PropertyImageUrl.Contains("http") ? [x.PropertyImageUrl] : [],
                        Description =
                            $"{x.Rooms!.Count} room{(x.Rooms.Count > 1 ? "s" : "")} at {x.PropertyName} from {x.CheckInDateParsed?.ToShortDateString()} - {x.CheckOutDateParsed?.ToShortDateString()}."
                    }
                }
            }) ?? Array.Empty<SessionLineItemOptions>());
        }

         var tourItems = new List<SessionLineItemOptions>();

        foreach (var item in basketLineItemsModels)
        {
            if (item.TourImageUrl != null && item.TourImageUrl.Contains("http"))
            {
                tourItems.Add(new SessionLineItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = item.Total.ConvertToCents(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.TourName,
                            Images = [item.TourImageUrl],
                            Description =
                                $"{item.GuestCount} guest{(item.GuestCount > 1 ? "s" : "")} at {item.TourName} on {string.Join(", ", item.TourDates.Select(td => td.StartDate.ToLongDateString()))}"
                        }
                    }
                });
            }
            else
            {
                tourItems.Add(new SessionLineItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = item.Total.ConvertToCents(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.TourName,
                            Description =
                                $"{item.GuestCount} guest{(item.GuestCount > 1 ? "s" : "")} at {item.TourName} on {string.Join(", ", item.TourDates.Select(td => td.StartDate.ToLongDateString()))}"
                        }
                    }
                });
            }
        }
        
        return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
            ? string.Join(", ", basketLineItemsModels.Select(x => x.TourName))
            : "", tourItems);
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
                    UnitAmount = x.TotalAmount.ConvertToCents(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = x.Property.Name,
                        Images = x.Property.ImagePath.Contains("http") ? [x.Property.ImagePath] : [],
                        Description =
                            $"{x.Rooms!.Count} room{(x.Rooms.Count > 1 ? "s" : "")} at {x.Property.Name} from {x.StartDate.ToShortDateString()} - {x.EndDate.ToShortDateString()}."
                    }
                }
            }) ?? Array.Empty<SessionLineItemOptions>());
        }

        var tourItems = new List<SessionLineItemOptions>();

        foreach (var item in basketLineItemsModels)
        {
            if (item.Tour.ImagePath.Contains("http"))
            {
                tourItems.Add(new SessionLineItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = item.TotalAmount.ConvertToCents(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Tour.Name,
                            Images = [item.Tour.ImagePath],
                            Description =
                                $"{item.Guests?.Count + 1} guest{(item.Guests?.Count + 1 > 1 ? "s" : "")} at {item.Tour.Name} on {string.Join(", ", item.StartDate.ToLongDateString())}"
                        }
                    }
                });
            }
            else
            {
                tourItems.Add(new SessionLineItemOptions()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = item.TotalAmount.ConvertToCents(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Tour.Name,
                            Description =
                                $"{item.Guests?.Count + 1} guest{(item.Guests?.Count + 1 > 1 ? "s" : "")} at {item.Tour.Name} on {string.Join(", ", item.StartDate.ToLongDateString())}"
                        }
                    }
                });
            }
        }
        
        return new Tuple<string, IEnumerable<SessionLineItemOptions>>(basketLineItemsModels != null
            ? string.Join(", ", basketLineItemsModels.Select(x => x.Tour.Name))
            : "", tourItems);
    }
}