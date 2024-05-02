using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingsByDateSpec : Specification<BookingItem, BookingItemDto>, ISingleResultSpecification<BookingItem>
{
    public BookingsByDateSpec(DefaultIdType tourDateId) =>
        Query.Where(p => p.TourDateId == tourDateId);
}

public class BookingsByDatesSpec : Specification<BookingItem, BookingItemDto>
{
    public BookingsByDatesSpec(IEnumerable<DefaultIdType> tourDateIds) =>
        Query.Where(p => p.TourDateId.HasValue && tourDateIds.Contains(p.TourDateId.Value));
}