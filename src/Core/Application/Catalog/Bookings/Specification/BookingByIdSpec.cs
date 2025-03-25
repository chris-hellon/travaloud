using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class BookingByIdSpec : Specification<Booking, BookingDetailsDto>, ISingleResultSpecification<Booking>
{
    public BookingByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Items).ThenInclude(x => x.Rooms)
            .Include(p => p.Items).ThenInclude(x => x.TourDate).ThenInclude(x => x.TourPrice)
            .Include(p => p.Items).ThenInclude(x => x.Guests)
            .AsSplitQuery();
}

public class BookingByIdWithDetailsSpec : Specification<Booking>, ISingleResultSpecification<Booking>
{
    public BookingByIdWithDetailsSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Items).ThenInclude(x => x.Rooms)
            .Include(p => p.Items).ThenInclude(x => x.TourDate).ThenInclude(x => x.TourPrice)
            .Include(p => p.Items).ThenInclude(x => x.Guests).AsSplitQuery();
}