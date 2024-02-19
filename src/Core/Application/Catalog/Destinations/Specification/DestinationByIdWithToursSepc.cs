using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Specification;

public class DestinationByIdWithToursSpec : Specification<Destination, DestinationDetailsDto>, ISingleResultSpecification<Destination>
{
    public DestinationByIdWithToursSpec(DefaultIdType id) =>
        Query
            .Where(p => p.Id == id);
}