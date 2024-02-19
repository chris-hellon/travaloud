using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Domain.Catalog.Events;

namespace Travaloud.Application.Catalog.Events.Specification;

public class EventsBySearchSpec : EntitiesByPaginationFilterSpec<Event, EventDto>
{
    public EventsBySearchSpec(SearchEventsRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.Name != null && p.Name.Equals(request.Name), request.Name != null);
}