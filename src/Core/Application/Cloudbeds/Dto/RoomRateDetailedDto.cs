namespace Travaloud.Application.Cloudbeds.Dto;

public class RoomRateDetailedDto
{
    [JsonProperty("date")] public DateTime Date { get; set; }
    [JsonProperty("rate")] public decimal Rate { get; set; }
    [JsonProperty("adults")] public int Adults { get; set; }
    [JsonProperty("kids")] public int Kids { get; set; }
}