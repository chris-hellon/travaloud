using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Specification;

public class TravelGuidesWithDetailsSpec : EntitiesByPaginationFilterSpec<TravelGuide, TravelGuideDto>
{
    public TravelGuidesWithDetailsSpec(SearchTravelGuidesRequest request)
        : base(request) =>
        Query
            .Include(x => x.TravelGuideGalleryImages)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .AsSplitQuery();
}