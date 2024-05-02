using Travaloud.Application.Catalog.PageSorting.Dto;

namespace Travaloud.Application.Catalog.PageSorting.Queries;

public class GetPageSortingsRequest : IRequest<IEnumerable<PageSortingDto>>
{
    
}

internal class GetPageSortingsRequestHandler : IRequestHandler<GetPageSortingsRequest, IEnumerable<PageSortingDto>>
{
    private readonly IRepositoryFactory<Domain.Catalog.Pages.PageSorting> _repository;

    public GetPageSortingsRequestHandler(IRepositoryFactory<Domain.Catalog.Pages.PageSorting> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PageSortingDto>> Handle(GetPageSortingsRequest request, CancellationToken cancellationToken)
    {
        var pageSortings = await _repository.ListAsync(cancellationToken);
        
        return pageSortings.Adapt<IEnumerable<PageSortingDto>>();
    }
}