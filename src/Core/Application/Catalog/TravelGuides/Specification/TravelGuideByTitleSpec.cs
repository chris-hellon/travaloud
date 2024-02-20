using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Specification;

public class TravelGuideByTitleSpec : Specification<TravelGuide>, ISingleResultSpecification<TravelGuide>
{
    public TravelGuideByTitleSpec(string name) =>
        Query.Where(p => p.Title == name);
}