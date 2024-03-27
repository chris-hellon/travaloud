using Travaloud.Application.Catalog.Destinations.Dto;

namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourDestinationLookupDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public DefaultIdType DestinationId { get; set; }
    public DestinationDto? Destination { get; set; }
}