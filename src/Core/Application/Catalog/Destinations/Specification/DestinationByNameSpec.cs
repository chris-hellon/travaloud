using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Specification;

public class DestinationByNameSpec : Specification<Destination>, ISingleResultSpecification<Destination>
{
    public DestinationByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}