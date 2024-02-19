namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CreateBookingItemRoomRequest
{
    public string RoomName { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
    public int Nights { get; set; } = default!;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string GuestFirstName { get; set; } = default!;
    public string GuestLastName { get; set; } = default!;
    public string CloudbedsGuestId { get; set; } = default!;
}