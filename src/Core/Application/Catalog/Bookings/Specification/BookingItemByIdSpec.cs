using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingItemByIdSpec : Specification<BookingItem, BookingItemDetailsDto>, ISingleResultSpecification<BookingItem>
{
    public BookingItemByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.TourDate);
}