using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalAmount
    {
        get
        {
            if (Guests != null && Guests.Count != 0) return Amount * (Guests.Count + 1);

            return Amount;
        }
    }

    public string Status => Cancelled.HasValue && Cancelled.Value ? "Cancelled" :
        NoShow.HasValue && NoShow.Value ? "No Show" :
        CheckedIn.HasValue && CheckedIn.Value ? "Checked In" : "Paid";
    
    public int? RoomQuantity { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public bool ShowDetails { get; set; }
    public bool ShowGuestDetails { get; set; } 
    public int ConcurrencyVersion { get; set; } 
    public string? PickupLocation { get; set; }
    public bool? WaiverSigned { get;  set; }
    public DefaultIdType? TourCategoryId { get; set; }
    public bool? NoShow { get; set; }
    public bool? Cancelled { get; set; }
    public bool? CheckedIn { get; set; }
    
    public TourDto? Tour { get; set; }
    public TourDateDto? TourDate { get; set; }
    public TourCategoryDto? TourCategory { get; set; }
    public PropertyDto? Property { get; set; }
    public List<BookingItemRoomDto>? Rooms { get; set; }
    public List<BookingItemGuestDto>? Guests { get; set; }
}