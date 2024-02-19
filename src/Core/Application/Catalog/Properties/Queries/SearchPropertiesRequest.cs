using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Queries;

public class SearchPropertiesRequest : PaginationFilter, IRequest<PaginationResponse<PropertyDto>>
{
    public string? Name { get; set; }
}

public class SearchPropertiesRequestHandler : IRequestHandler<SearchPropertiesRequest, PaginationResponse<PropertyDto>>
{
    private readonly IRepositoryFactory<Property> _repository;

    public SearchPropertiesRequestHandler(IRepositoryFactory<Property> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<PropertyDto>> Handle(SearchPropertiesRequest request, CancellationToken cancellationToken)
    {
        var spec = new PropertiesBySearchSpec(request);

        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}