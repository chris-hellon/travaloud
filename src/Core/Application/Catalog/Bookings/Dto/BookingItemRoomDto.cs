namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemRoomDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingItemId { get; set; }
    public string RoomName { get; set; } = default!;
    public decimal Amount { get; set; }
    public int Nights { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string GuestFirstName { get; set; } = default!;
    public string GuestLastName { get; set; } = default!;
    public string CloudbedsGuestId { get; set; } = default!;
}