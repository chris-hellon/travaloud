using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourDatesByTourIdSpec : EntitiesByBaseFilterSpec<TourDate, TourDateDto>
{
    public TourDatesByTourIdSpec(GetTourDatesRequest request)
        : base(request) =>
        Query
            .Include(x => x.TourPrice)
            .OrderBy(c => c.StartDate)
            .Where(p => p.TourId == request.TourId)
            .Where(p => p.StartDate > DateTime.Now);
    // .Where(x => x.AvailableSpaces > 0 && x.AvailableSpaces >= request.RequestedSpaces);
}

public class TourDatesByTourIdNoLimitSpec : Specification<TourDate, TourDateDto>
{
    public TourDatesByTourIdNoLimitSpec(DefaultIdType tourId) =>
        Query
            .Include(x => x.TourPrice)
            .OrderBy(c => c.StartDate)
            .Where(p => p.TourId == tourId);
    // .Where(x => x.AvailableSpaces > 0 && x.AvailableSpaces >= request.RequestedSpaces);
}