using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourByNameSpec : Specification<Tour>, ISingleResultSpecification<Tour>
{
    public TourByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}