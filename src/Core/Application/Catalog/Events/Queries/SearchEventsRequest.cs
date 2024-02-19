using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Specification;
using Travaloud.Domain.Catalog.Events;

namespace Travaloud.Application.Catalog.Events.Queries;

public class SearchEventsRequest : PaginationFilter, IRequest<PaginationResponse<EventDto>>
{
    public string? Name { get; set; }
}

public class SearchEventsRequestHandler : IRequestHandler<SearchEventsRequest, PaginationResponse<EventDto>>
{
    private readonly IRepositoryFactory<Event> _repository;

    public SearchEventsRequestHandler(IRepositoryFactory<Event> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<EventDto>> Handle(SearchEventsRequest request,
        CancellationToken cancellationToken)
    {
        var spec = new EventsBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize,
            cancellationToken: cancellationToken);
    }
}