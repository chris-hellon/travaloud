using Travaloud.Application.Cloudbeds.Dto;

namespace Travaloud.Application.Cloudbeds.Responses;

public class GetPropertyAvailabilityResponse : CloudbedsResponse<IEnumerable<PropertyAvailabilityDto>>
{
    [JsonProperty("roomCount")] public int RoomCount { get; set; }
}