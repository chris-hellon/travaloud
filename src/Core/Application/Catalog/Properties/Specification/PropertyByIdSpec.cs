using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertyByIdSpec : Specification<Property, PropertyDetailsDto>, ISingleResultSpecification<Property>
{
    public PropertyByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id)
            .Include(p => p.Directions).ThenInclude(x => x.Content)
            .Include(p => p.Rooms)
            .Include(p => p.Facilities)
            .Include(p => p.Images);

        Query.Include(p => p.PropertyDestinationLookups)
            .ThenInclude(x => x.Destination)
            .Where(p => p.PropertyDestinationLookups != null);
    }
}