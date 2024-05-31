using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class TourBookingsCountSpec : Specification<Booking, BookingDetailsDto>
{
    public TourBookingsCountSpec(DateTime? fromDate, DateTime? toDate) =>
        Query.Where(p => p.Items.Any(x => x.TourId.HasValue))
            .Where(b => b.BookingDate >= fromDate.Value && b.BookingDate <= toDate.Value, condition: fromDate.HasValue && toDate.HasValue)
            .Include(x => x.Items);
}
