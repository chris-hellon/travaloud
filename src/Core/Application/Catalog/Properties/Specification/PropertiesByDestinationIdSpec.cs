using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertiesByDestinationIdSpec : Specification<Property, PropertyDto>
{
    public PropertiesByDestinationIdSpec(DefaultIdType id)
    {
        Query.Include(p => p.PropertyDestinationLookups)
            .ThenInclude(x => x.Destination)
            .Where(p => p.PropertyDestinationLookups != null &&
                        p.PropertyDestinationLookups.Any(x => x.DestinationId == id)).AsSplitQuery();
    }
}