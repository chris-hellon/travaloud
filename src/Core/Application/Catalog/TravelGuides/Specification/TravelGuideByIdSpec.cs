using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Specification;

public class TravelGuideByIdSpec : Specification<TravelGuide, TravelGuideDto>, ISingleResultSpecification<TravelGuide>
{
    public TravelGuideByIdSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.TravelGuideGalleryImages);
}