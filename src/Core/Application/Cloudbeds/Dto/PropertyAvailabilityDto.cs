namespace Travaloud.Application.Cloudbeds.Dto;

public class PropertyAvailabilityDto
{
    [JsonProperty("propertyID")] public int PropertyId { get; set; }

    [JsonProperty("propertyCurrency")] public PropertyCurrencyDto PropertyCurrency { get; set; } = default!;

    [JsonProperty("propertyRooms")] public List<PropertyRoomDto> PropertyRooms { get; set; } = default!;
}