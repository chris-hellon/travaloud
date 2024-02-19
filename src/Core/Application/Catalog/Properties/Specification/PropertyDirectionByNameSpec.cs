using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertyDirectionByNameSpec : Specification<PropertyDirection>, ISingleResultSpecification<PropertyDirection>
{
    public PropertyDirectionByNameSpec(string title, DefaultIdType propertyId) =>
        Query.Where(p => p.Title == title && p.PropertyId == propertyId);
}