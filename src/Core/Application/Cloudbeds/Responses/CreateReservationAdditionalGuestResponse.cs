namespace Travaloud.Application.Cloudbeds.Responses;

public class CreateReservationAdditionalGuestResponse
{
    [JsonProperty("success")] public bool Success { get; set; }
    
    [JsonProperty("guestID")] public int GuestId { get; set; }
}