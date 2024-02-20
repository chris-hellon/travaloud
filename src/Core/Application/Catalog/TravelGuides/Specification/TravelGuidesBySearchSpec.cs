using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Specification;

public class TravelGuidesBySearchSpec : EntitiesByPaginationFilterSpec<TravelGuide, TravelGuideDto>
{
    public TravelGuidesBySearchSpec(SearchTravelGuidesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.Title.Equals(request.Title), request.Title != null)
            .Where(p => p.CreatedOn.Equals(request.CreatedOn), request.CreatedOn != null)
            .Where(p => p.CreatedBy.Equals(request.CreatedBy), request.CreatedBy != null);
}