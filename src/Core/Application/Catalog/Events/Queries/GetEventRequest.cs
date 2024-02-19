using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Specification;
using Travaloud.Domain.Catalog.Events;

namespace Travaloud.Application.Catalog.Events.Queries;

public class GetEventRequest : IRequest<EventDto>
{
    public GetEventRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetEventRequestHandler : IRequestHandler<GetEventRequest, EventDto>
{
    private readonly IRepositoryFactory<Event> _repository;
    private readonly IStringLocalizer<GetEventRequestHandler> _localizer;

    public GetEventRequestHandler(IRepositoryFactory<Event> repository, IStringLocalizer<GetEventRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<EventDto> Handle(GetEventRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new EventByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["event.notfound"], request.Id));
}