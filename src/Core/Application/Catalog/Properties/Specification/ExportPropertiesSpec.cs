using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Specification;

public class ExportPropertiesSpec : EntitiesByBaseFilterSpec<Property, PropertyExportDto>
{
    public ExportPropertiesSpec(ExportPropertiesRequest request)
        : base(request) =>
        Query.Where(d => d.Name == request!.Name);
}