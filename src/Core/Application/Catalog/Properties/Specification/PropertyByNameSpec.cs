using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertyByNameSpec : Specification<Property>, ISingleResultSpecification<Property>
{
    public PropertyByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}