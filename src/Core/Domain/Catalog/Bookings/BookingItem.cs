using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Domain.Catalog.Bookings;

public class BookingItem : AuditableEntity, IAggregateRoot
{
    public DefaultIdType BookingId { get; private set; } = default!;
    public DateTime StartDate { get; private set; } = default!;
    public DateTime? EndDate { get; private set; }
    public decimal Amount { get; private set; } = default!;
    public int? RoomQuantity { get; set; }
    public DefaultIdType? PropertyId { get; private set; }
    public DefaultIdType? TourId { get; private set; }
    public DefaultIdType? TourCategoryId { get; private set; }
    public DefaultIdType? TourDateId { get; private set; }
    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public int ConcurrencyVersion { get; set; }
    public string? PickupLocation { get; set; }
    public bool? WaiverSigned { get; private set; }
    public bool? Cancelled { get; private set; }
    public bool? NoShow { get; private set; }
    public bool? CheckedIn { get; private set; }
    
    public virtual IList<BookingItemRoom>? Rooms { get; set; }
    public virtual Tour? Tour { get; set; }
    public virtual TourCategory? TourCategory { get; set; }
    public virtual TourDate? TourDate { get; set; }
    public virtual Property? Property { get; set; }
    public virtual Booking Booking { get; private set; } = default!;
    public virtual IList<BookingItemGuest>? Guests { get; set; }
    
    public BookingItem(
        DateTime startDate,
        DateTime? endDate,
        decimal amount,
        int? roomQuantity,
        DefaultIdType? propertyId,
        DefaultIdType? tourId,
        DefaultIdType? tourDateId,
        string? cloudbedsReservationId,
        int? cloudbedsPropertyId,
        string? pickupLocation,
        bool? waiverSigned,
        DefaultIdType? tourCategoryId,
        DefaultIdType createdBy)
    {
        StartDate = startDate;
        EndDate = endDate;
        Amount = amount;
        RoomQuantity = roomQuantity;
        PropertyId = propertyId;
        TourId = tourId;
        TourDateId = tourDateId;
        CloudbedsReservationId = cloudbedsReservationId;
        CloudbedsPropertyId = cloudbedsPropertyId;
        PickupLocation = pickupLocation;
        WaiverSigned = waiverSigned;
        TourCategoryId = tourCategoryId;
        CreatedBy = createdBy;
    }

    public BookingItem Update(
        DateTime? startDate,
        DateTime? endDate,
        decimal? amount,
        int? roomQuantity,
        DefaultIdType? propertyId,
        DefaultIdType? tourId,
        DefaultIdType? tourDateId,
        string? cloudbedsReservationId,
        int? cloudbedsPropertyId,
        IList<BookingItemRoom>? rooms,
        string? pickupLocation,
        bool? waiverSigned,
        DefaultIdType? tourCategoryId,
        DefaultIdType createdBy,
        bool? checkedIn,
        bool? cancelled,
        bool? noShow)
    {
        if (startDate is not null && StartDate != startDate)
            StartDate = startDate.Value;

        if (endDate is not null && EndDate != endDate)
            EndDate = endDate.Value;

        if (amount is not null && Amount != amount)
            Amount = amount.Value;

        if (roomQuantity is not null && RoomQuantity != roomQuantity)
            RoomQuantity = roomQuantity;

        if (propertyId is not null && PropertyId != propertyId)
            PropertyId = propertyId;

        if (tourId is not null && TourId != tourId)
            TourId = tourId;

        if (tourDateId is not null && TourDateId != tourDateId)
            TourDateId = tourDateId;

        if (cloudbedsReservationId is not null && CloudbedsReservationId != cloudbedsReservationId)
            CloudbedsReservationId = cloudbedsReservationId;

        if (cloudbedsPropertyId is not null && CloudbedsPropertyId != cloudbedsPropertyId)
            CloudbedsPropertyId = cloudbedsPropertyId;

        if (rooms is not null && Rooms != rooms)
            Rooms = rooms;
        
        if (checkedIn is not null && CheckedIn != checkedIn)
            CheckedIn = checkedIn.Value;
        
        if (cancelled is not null && Cancelled != cancelled)
            Cancelled = cancelled.Value;
        
        if (noShow is not null && NoShow != noShow)
            NoShow = noShow.Value;
        
        PickupLocation = pickupLocation;
        WaiverSigned = waiverSigned;
        TourCategoryId = tourCategoryId;
        CreatedBy = createdBy;
        
        return this;
    }

    public BookingItem SetReservationId(string reservationId)
    {
        CloudbedsReservationId = reservationId;

        return this;
    }

    public BookingItem SetEndDate(DateTime endDate)
    {
        EndDate = endDate;

        return this;
    }
    
    public BookingItem AddGuest(BookingItemGuest guest)
    {
        Guests ??= new List<BookingItemGuest>();
        Guests.Add(guest);

        return this;
    }
}