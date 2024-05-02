using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Specification;

public class TravelGuideByFriendlyTitleSpec : Specification<TravelGuide, TravelGuideDetailsDto>, ISingleResultSpecification<TravelGuide>
{
    public TravelGuideByFriendlyTitleSpec(string title) =>
        Query
            .Where(p => p.UrlFriendlyTitle == title)
            .Include(p => p.TravelGuideGalleryImages);
}