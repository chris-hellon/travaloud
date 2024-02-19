using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingByGuestIdAndDateSpec : Specification<Booking>, ISingleResultSpecification<Booking>
{
    public BookingByGuestIdAndDateSpec(string? guestId, DateTime bookingDate, string description) =>
        Query.Where(p => guestId != null && p.GuestId == guestId && p.BookingDate == bookingDate && p.Description == description);
}