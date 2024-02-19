using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class SearchTourCategoriesRequest : PaginationFilter, IRequest<PaginationResponse<TourCategoryDto>>
{
    public string? Name { get; set; }
    public bool? IsTopLevel { get; set; } = true;
}

public class SearchTourCategoriesRequestRequestHandler : IRequestHandler<SearchTourCategoriesRequest, PaginationResponse<TourCategoryDto>>
{
    private readonly IRepositoryFactory<TourCategory> _repository;

    public SearchTourCategoriesRequestRequestHandler(IRepositoryFactory<TourCategory> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<TourCategoryDto>> Handle(SearchTourCategoriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new TourCategoriesBySearchSpec(request);

        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}