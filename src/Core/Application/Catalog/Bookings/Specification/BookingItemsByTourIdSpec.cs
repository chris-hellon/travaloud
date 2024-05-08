using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingItemsByTourIdSpec : Specification<BookingItem>
{
    public BookingItemsByTourIdSpec(DefaultIdType tourId) =>
        Query.Where(p => p.TourId.HasValue && p.TourId.Value == tourId && p.StartDate >= DateTime.Now);
}