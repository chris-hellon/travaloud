using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class PropertyBookingsCountSpec : Specification<Booking>
{
    public PropertyBookingsCountSpec() =>
        Query.Where(p => p.Items.Any(x => x.PropertyId.HasValue));
}
