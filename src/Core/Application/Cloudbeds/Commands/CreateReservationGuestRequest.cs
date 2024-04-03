namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationGuestRequest
{
    [JsonProperty("roomTypeID")]
    public string RoomTypeId { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    // [JsonProperty("roomID")]
    // public string? RoomId { get; set; }
}