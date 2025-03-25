using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Specification;

public class SameTourDatesSpec : Specification<TourDate>
{
    public SameTourDatesSpec(DefaultIdType tourId, DateTime startDate, DateTime endDate, DefaultIdType tourDateId,
        List<DefaultIdType>? tourDateIdsUsed = null) =>
        Query
            .Where(p => p.Id != tourDateId && p.TourId == tourId && p.StartDate == startDate &&
                        p.EndDate == endDate); // Apply sorting after filtering
}