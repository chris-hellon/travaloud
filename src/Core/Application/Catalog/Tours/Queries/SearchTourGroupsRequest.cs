using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class SearchTourGroupsRequest : PaginationFilter, IRequest<PaginationResponse<TourCategoryDto>>
{
    public string? Name { get; set; }
    public bool? IsTopLevel { get; set; } = null;
}

public class SearchTourGroupsRequestRequestHandler : IRequestHandler<SearchTourGroupsRequest, PaginationResponse<TourCategoryDto>>
{
    private readonly IRepositoryFactory<TourCategory> _repository;

    public SearchTourGroupsRequestRequestHandler(IRepositoryFactory<TourCategory> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<TourCategoryDto>> Handle(SearchTourGroupsRequest request, CancellationToken cancellationToken)
    {
        var spec = new TourCategoriesBySearchSpec(new SearchTourCategoriesRequest()
        {
            AdvancedSearch = request.AdvancedSearch,
            IsTopLevel = false,
            Keyword = request.Keyword,
            Name = request.Name,
            OrderBy = request.OrderBy,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });

        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}