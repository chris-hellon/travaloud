using System.Text.RegularExpressions;
using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Queries;
using PropertyRoomDto = Travaloud.Application.Cloudbeds.Dto.PropertyRoomDto;

namespace Travaloud.Application.Common.Extensions;

public static class CloudbedsExtensions
{
    public static string ReplaceFunkyFirstnames(this string firstName)
    {
        const string pattern = @"\[[^\]]*\]|\([^()]*\)";
        const string replacement = ""; // Replace with empty string
        return Regex.Replace(firstName, pattern, replacement);
    }
    
    public static int GetMaximumOccupancy(this PropertyRoomDto propertyRoom)
    {
        if (!propertyRoom.IsSharedRoom)
            return int.Parse(propertyRoom.MaxGuests);

        const string pattern = @"\d+";
        var match = Regex.Match(propertyRoom.RoomTypeName, pattern);

        return match.Success ? int.Parse(match.Value) : 0;
    }

    public static async Task<BasketModel> CheckCloudbedsReservations(this List<string> errorMessages,
        BasketModel basket,
        ITenantWebsiteService tenantWebsiteService,
        ICloudbedsService cloudbedsService,
        IBasketService basketService)
    {
        // We need to check if availability has changed since the session was created 
        if (!basket.Items.Any(x => x.PropertyId.HasValue)) return basket;
        {
            var properties = await tenantWebsiteService.GetProperties(new CancellationToken());
            if (properties == null) return basket;
            var propertyBasketItems = basket.Items.Where(x => x.PropertyId.HasValue);
            
            foreach (var basketItem in propertyBasketItems)
            {
                if (basketItem.Rooms != null)
                {
                    var propertyDtos = properties as PropertyDto[] ?? properties.ToArray();
                    var property = propertyDtos.FirstOrDefault(x =>
                        basketItem.PropertyId != null && x.Id == basketItem.PropertyId.Value);

                    var cloudbedsAvailabilityResponse = await cloudbedsService.GetPropertyAvailability(
                        new GetPropertyAvailabilityRequest()
                        {
                            StartDate = basketItem.CheckInDateParsed?.ToString("yyyy-MM-dd") ?? string.Empty,
                            EndDate = basketItem.CheckOutDateParsed?.ToString("yyyy-MM-dd") ?? string.Empty,
                            PropertyId = property?.CloudbedsPropertyId ?? string.Empty,
                            PropertyApiKey = property?.CloudbedsApiKey ?? string.Empty
                        });

                    if (cloudbedsAvailabilityResponse is {Success: true, Data: not null} &&
                        cloudbedsAvailabilityResponse.Data.Any())
                    {
                        var propertyData = cloudbedsAvailabilityResponse.Data.First();

                        var propertyRoomTypeIds = basketItem.Rooms.Select(x => x.RoomTypeId);
                        var existingRooms =
                            propertyData.PropertyRooms.Where(x => propertyRoomTypeIds.Contains(x.RoomTypeId));
                        var propertyRoomDtos = existingRooms as PropertyRoomDto[] ?? existingRooms.ToArray();

                        if (propertyRoomDtos.Length != 0)
                        {
                            foreach (var basketRoomItem in basketItem.Rooms)
                            {
                                var existingRoom =
                                    propertyRoomDtos.FirstOrDefault(x => x.RoomTypeId == basketRoomItem.RoomTypeId);

                                if (existingRoom == null)
                                {
                                    errorMessages.Add($"{basketItem.PropertyName} - {basketRoomItem.RoomTypeName} currently has no availability, please select an alternative room.");
                                    basket = await basketService.RemoveRoom(basketRoomItem.Id, basketItem.Id);
                                    continue;
                                }

                                if (existingRoom.RoomsAvailable >= basketRoomItem.RoomQuantity) continue;

                                errorMessages.Add($"{basketItem.PropertyName} - {basketRoomItem.RoomTypeName} currently only has {existingRoom.RoomsAvailable} room(s) available, amend your selection.");
                                basket = await basketService.RemoveRoom(basketRoomItem.Id, basketItem.Id);
                            }
                        }
                        else
                        {
                            errorMessages.Add($"{basketItem.PropertyName} currently has no availability, please select alternative dates.");
                            basket = await basketService.RemoveItem(basketItem.Id);
                        }
                    }
                    else
                    {
                        errorMessages.Add($"{basketItem.PropertyName} currently has no availability, please select alternative dates.");
                        basket = await basketService.RemoveItem(basketItem.Id);
                    }
                }
                else errorMessages.Add($"{basketItem.PropertyName} currently has no rooms selected.");
            }
        }

        return basket;
    }
}