using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourDateByDatesSpec : Specification<TourDate>, ISingleResultSpecification<TourDate>
{
    public TourDateByDatesSpec(DateTime? startDate, DateTime? endDate, DefaultIdType tourId) =>
        Query.Where(p => p.TourId == tourId && p.StartDate == startDate && p.EndDate == endDate);
}