using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourCategoryByNameSpec : Specification<TourCategory>, ISingleResultSpecification<TourCategory>
{
    public TourCategoryByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}