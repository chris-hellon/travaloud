using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourDatesByTourIdSpec : EntitiesByBaseFilterSpec<TourDate, TourDateDto>
{
    public TourDatesByTourIdSpec(GetTourDatesRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.StartDate)
            .Where(p => p.TourId == request.TourId).Include(x => x.TourPrice);
    // .Where(x => x.AvailableSpaces > 0 && x.AvailableSpaces >= request.RequestedSpaces);
}