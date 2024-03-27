using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Specification;

public class GetDestinationsSpec : Specification<Destination, DestinationDto>
{
    public GetDestinationsSpec(GetDestinationsRequest request) =>
        Query
            .OrderBy(c => c.Name);
}