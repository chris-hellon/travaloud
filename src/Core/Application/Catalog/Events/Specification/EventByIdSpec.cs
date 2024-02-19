using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Domain.Catalog.Events;

namespace Travaloud.Application.Catalog.Events.Specification;

public class EventByIdSpec : Specification<Event, EventDto>, ISingleResultSpecification<Event>
{
    public EventByIdSpec(DefaultIdType id)
    {
            Query.Where(p => p.Id == id);
        }
}