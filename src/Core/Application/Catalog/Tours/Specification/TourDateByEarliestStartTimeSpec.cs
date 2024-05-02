using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourDateByEarliestStartTimeSpec : Specification<TourDate>, ISingleResultSpecification<TourDate>
{
    public TourDateByEarliestStartTimeSpec(DefaultIdType tourId) =>
        Query
            .Include(x => x.TourPrice)
            .Where(p => p.Id == tourId)
            .Where(p => p.StartDate > DateTime.Now)
            .OrderBy(p => p.StartDate);
}