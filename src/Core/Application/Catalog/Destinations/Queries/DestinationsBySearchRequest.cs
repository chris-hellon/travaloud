using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Queries;

public class DestinationsBySearchRequest : EntitiesByPaginationFilterSpec<Destination, DestinationDto>
{
    public DestinationsBySearchRequest(SearchDestinationsRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.Name.Equals(request.Name), request.Name != null);
}

