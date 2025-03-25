using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Specification;

public class TourDatesByTourIdNoLimitSpec : Specification<TourDate, TourDateDto>
{
    public TourDatesByTourIdNoLimitSpec(DefaultIdType tourId) =>
        Query
            .Include(x => x.TourPrice)
            .OrderBy(c => c.StartDate)
            .Where(p => p.TourId == tourId)
            .AsSplitQuery();
    // .Where(x => x.AvailableSpaces > 0 && x.AvailableSpaces >= request.RequestedSpaces);
}

public class TourDatesByTourIdsWithinRangeSpec : Specification<TourDate, TourDateDto>
{
    public TourDatesByTourIdsWithinRangeSpec(List<DefaultIdType> tourIds, DateTime fromDate, DateTime toDate) =>
        Query
            .Include(x => x.TourPrice)
            .OrderBy(c => c.StartDate)
            .Where(p => tourIds.Contains(p.TourId) && p.StartDate >= fromDate && p.EndDate <= toDate)
            .AsSplitQuery();
}