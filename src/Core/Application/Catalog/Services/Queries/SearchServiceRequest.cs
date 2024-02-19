using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Queries;

public class SearchServiceRequest : PaginationFilter, IRequest<PaginationResponse<ServiceDto>>
{
    public string? Name { get; set; }
}

public class SearchServiceRequestHandler : IRequestHandler<SearchServiceRequest, PaginationResponse<ServiceDto>>
{
    private readonly IRepositoryFactory<Service> _repository;

    public SearchServiceRequestHandler(IRepositoryFactory<Service> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<ServiceDto>> Handle(SearchServiceRequest request, CancellationToken cancellationToken)
    {
        var spec = new ServicesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}