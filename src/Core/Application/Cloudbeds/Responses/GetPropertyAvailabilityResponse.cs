using Travaloud.Application.Cloudbeds.Dto;

namespace Travaloud.Application.Cloudbeds.Responses;

public class GetPropertyAvailabilityResponse : CloudbedsResponse<PropertyAvailabilityDto>
{
    [JsonProperty("roomCount")] public int RoomCount { get; set; }
}