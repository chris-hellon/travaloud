using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertiesBySearchSpec : EntitiesByPaginationFilterSpec<Property, PropertyDto>
{
    public PropertiesBySearchSpec(SearchPropertiesRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}