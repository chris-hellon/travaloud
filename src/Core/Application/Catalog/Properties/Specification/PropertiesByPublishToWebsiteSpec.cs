using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class PropertiesByPublishToWebsiteSpec : Specification<Property, PropertyDto>
{
    public PropertiesByPublishToWebsiteSpec(GetPropertiesByPublishToWebsiteRequest request) =>
        Query
            .OrderBy(c => c.Name)
            .Where(p => p.PublishToSite.HasValue && p.PublishToSite.Value);
}