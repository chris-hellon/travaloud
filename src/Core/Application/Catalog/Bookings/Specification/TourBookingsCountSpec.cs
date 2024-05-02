using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class TourBookingsCountSpec : Specification<Booking, BookingDetailsDto>
{
    public TourBookingsCountSpec() =>
        Query.Where(p => p.Items.Any(x => x.TourId.HasValue))
            .Include(x => x.Items);
}
