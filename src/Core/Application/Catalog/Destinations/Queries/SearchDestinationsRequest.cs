using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Queries;

public class SearchDestinationsRequest : PaginationFilter, IRequest<PaginationResponse<DestinationDto>>
{
    public string? Name { get; set; }
}

public class SearchDestinationsRequestHandler : IRequestHandler<SearchDestinationsRequest, PaginationResponse<DestinationDto>>
{
    private readonly IRepositoryFactory<Destination> _repository;

    public SearchDestinationsRequestHandler(IRepositoryFactory<Destination> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<DestinationDto>> Handle(SearchDestinationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new DestinationsBySearchRequest(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}