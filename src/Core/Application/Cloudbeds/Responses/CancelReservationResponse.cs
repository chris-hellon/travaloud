namespace Travaloud.Application.Cloudbeds.Responses;

public class CancelReservationResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
}