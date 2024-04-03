namespace Travaloud.Application.Cloudbeds.Responses;

public class CreateReservationResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("reservationID")]
    public string? ReservationID { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("guestID")]
    public string? GuestId { get; set; }

    [JsonProperty("guestFirstName")]
    public string? GuestFirstName { get; set; }

    [JsonProperty("guestLastName")]
    public string? GuestLastName { get; set; }

    [JsonProperty("guestGender")]
    public string? GuestGender { get; set; }

    [JsonProperty("guestEmail")]
    public string? GuestEmail { get; set; }

    [JsonProperty("startDate")]
    public DateTime StartDate { get; set; }

    [JsonProperty("endDate")]
    public DateTime EndDate { get; set; }

    [JsonProperty("dateCreated")]
    public DateTime DateCreated { get; set; }

    [JsonProperty("grandTotal")]
    public float GrandTotal { get; set; }

    [JsonProperty("unassigned")]
    public UnassignedRoom[]? Unassigned { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}

public class UnassignedRoom
{
    [JsonProperty("subReservationID")]
    public string? SubReservationId { get; set; }

    [JsonProperty("roomTypeName")]
    public string? RoomTypeName { get; set; }

    [JsonProperty("roomTypeID")]
    public int? RoomTypeId { get; set; }

    [JsonProperty("adults")]
    public int Adults { get; set; }

    [JsonProperty("children")]
    public int Children { get; set; }

    [JsonProperty("dailyRates")]
    public DailyRate[]? DailyRates { get; set; }

    [JsonProperty("roomTotal")]
    public float RoomTotal { get; set; }
}

public class DailyRate
{
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("rate")]
    public float Rate { get; set; }
}
