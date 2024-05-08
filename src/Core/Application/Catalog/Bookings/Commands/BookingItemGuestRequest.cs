using Travaloud.Application.Identity.Users;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class BookingItemGuestRequest
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingItemId { get; set; }
    public string? GuestId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public UserDetailsDto? Guest { get; set; }
}