using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertyRoomByNameSpec : Specification<PropertyRoom>, ISingleResultSpecification<PropertyRoom>
{
    public PropertyRoomByNameSpec(string name, DefaultIdType propertyId) =>
        Query.Where(p => p.Name == name && p.PropertyId == propertyId);
}