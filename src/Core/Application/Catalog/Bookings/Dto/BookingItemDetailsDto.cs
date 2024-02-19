using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingItemDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType BookingId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int? RoomQuantity { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourDateId { get; set; }
    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public bool ShowDetails { get; set; } = default!;
    public int ConcurrencyVersion { get; set; } = default!;

    public TourDto? Tour { get; set; }
    public TourDateDto? TourDate { get; set; }
    public PropertyDto? Property { get; set; }
    public List<BookingItemRoomDto>? Rooms { get; set; }
}