using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingItemsByDateSpec : Specification<BookingItem>
{
    public BookingItemsByDateSpec(DateTime startDate) =>
        Query.Where(p => p.StartDate.Date == startDate.Date && p.TourDateId.HasValue)
            .Include(x => x.Tour)
            .Include(x => x.TourDate);
}