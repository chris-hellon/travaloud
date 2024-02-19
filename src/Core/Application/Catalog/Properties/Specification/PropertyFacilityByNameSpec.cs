using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertyFacilityByNameSpec : Specification<PropertyFacility>, ISingleResultSpecification<PropertyFacility>
{
    public PropertyFacilityByNameSpec(string title, DefaultIdType propertyId) =>
        Query.Where(p => p.Title == title && p.PropertyId == propertyId);
}