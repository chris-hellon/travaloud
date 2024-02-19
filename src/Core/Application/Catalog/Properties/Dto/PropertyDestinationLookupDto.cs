using Travaloud.Application.Catalog.Destinations.Dto;

namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyDestinationLookupDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public DefaultIdType DestinationId { get; set; }
    public DestinationDto Destination { get; set; } = default!;
}