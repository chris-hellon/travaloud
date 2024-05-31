using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class ToursByPublishToWebsiteSpec : Specification<Tour, TourDto>
{
    public ToursByPublishToWebsiteSpec(GetToursByPublishToWebsiteRequest request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy());
    //.Where(p => p.PublishToSite.HasValue && p.PublishToSite.Value);
}