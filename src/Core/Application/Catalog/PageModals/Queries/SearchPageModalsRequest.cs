using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Queries;

public class SearchPageModalsRequest : PaginationFilter, IRequest<PaginationResponse<PageModalDto>>
{
    public string? Title { get; set; }
}

public class SearchPageModalsRequestHandler : IRequestHandler<SearchPageModalsRequest, PaginationResponse<PageModalDto>>
{
    private readonly IRepositoryFactory<PageModal> _repository;

    public SearchPageModalsRequestHandler(IRepositoryFactory<PageModal> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<PageModalDto>> Handle(SearchPageModalsRequest request, CancellationToken cancellationToken)
    {
        var spec = new PageModalsBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}