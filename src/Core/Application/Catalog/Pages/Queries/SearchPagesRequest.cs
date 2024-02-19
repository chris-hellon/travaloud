using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Queries;

public class SearchPagesRequest : PaginationFilter, IRequest<PaginationResponse<PageDto>>
{
    public string? Title { get; set; }
}

public class SearchPagesRequestHandler : IRequestHandler<SearchPagesRequest, PaginationResponse<PageDto>>
{
    private readonly IRepositoryFactory<Page> _repository;

    public SearchPagesRequestHandler(IRepositoryFactory<Page> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<PageDto>> Handle(SearchPagesRequest request, CancellationToken cancellationToken)
    {
        var spec = new PagesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}