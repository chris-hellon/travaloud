namespace Travaloud.Application.Cloudbeds.Responses;

public class GetDashboardResponse
{
    [JsonProperty("roomsOccupied")] public int RoomsOccupied { get; set; }
    [JsonProperty("percentageOccupied")] public double PercentageOccupied { get; set; }
    [JsonProperty("arrivals")] public int Arrivals { get; set; }
    [JsonProperty("departures")] public int Departures { get; set; }
    [JsonProperty("inHouse")] public int InHouse { get; set; }
}