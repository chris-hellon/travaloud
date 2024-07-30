namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemGuestDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingItemId { get; set; }
    public DefaultIdType GuestId { get; set; }

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public bool? NoShow { get; set; }
    public bool? Cancelled { get; set; }
    public bool? CheckedIn { get; set; }
    
    public string Status => Cancelled.HasValue && Cancelled.Value ? "Cancelled" :
        NoShow.HasValue && NoShow.Value ? "No Show" :
        CheckedIn.HasValue && CheckedIn.Value ? "Checked In" : "Paid";
}