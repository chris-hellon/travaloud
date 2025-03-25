using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Specification;

public class SearchTourDatesSpec : EntitiesByPaginationFilterSpec<TourDate, TourDateDto>
{
    public SearchTourDatesSpec(SearchTourDatesRequest request)
        : base(request) =>
        Query
            .Include(x => x.TourPrice)
            .OrderBy(c => c.StartDate)
            .Where(p => p.StartDate > DateTime.Now, condition: !request.UserIsAdmin)
            .Where(p => p.StartDate > DateTime.Now.AddMonths(-1), condition: request.UserIsAdmin)
            .Where(x => x.EndDate <= request.EndDate.Value, condition: request.EndDate.HasValue)
            .Where(x => x.StartDate <= request.StartDate.Value, condition: request.StartDate.HasValue)
            .Where(p => p.TourId == request.TourId, condition: request.TourId.HasValue)
            .Where(p => p.TourPriceId == request.PriceId, condition: request.PriceId.HasValue)
            .Where(p => p.AvailableSpaces >= request.RequestedSpaces, condition: request.RequestedSpaces.HasValue)
            .AsSplitQuery();
}