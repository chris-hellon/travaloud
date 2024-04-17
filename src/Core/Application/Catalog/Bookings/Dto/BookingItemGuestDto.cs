namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemGuestDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingItemId { get; set; }
    public DefaultIdType GuestId { get; set; }
}