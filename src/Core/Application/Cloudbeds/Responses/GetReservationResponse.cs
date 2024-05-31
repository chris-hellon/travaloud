namespace Travaloud.Application.Cloudbeds.Responses;

public class GetReservationResponse
{
    [JsonProperty("balance")] public decimal Balance { get; set; }
}