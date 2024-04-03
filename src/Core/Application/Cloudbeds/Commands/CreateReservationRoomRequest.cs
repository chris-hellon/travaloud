namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationRoomRequest
{
    [JsonProperty("roomTypeID")]
    public string? RoomTypeId { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    // [JsonProperty("roomID")]
    // public int? RoomId { get; set; }
    //
    // [JsonProperty("roomRateID")]
    // public int? RoomRateId { get; set; }
}