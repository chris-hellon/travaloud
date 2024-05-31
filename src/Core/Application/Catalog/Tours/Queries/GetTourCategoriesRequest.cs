using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourCategoriesRequest : IRequest<IEnumerable<TourCategoryDto>>
{
    public bool ParentOnly { get; set; }

    public GetTourCategoriesRequest(bool parentOnly = false)
    {
        ParentOnly = parentOnly;
    }
}

public class GetTourCategoriesRequestHandler : IRequestHandler<GetTourCategoriesRequest, IEnumerable<TourCategoryDto>>
{
    private readonly IStringLocalizer<GetTourCategoriesRequestHandler> _localizer;
    private readonly IRepositoryFactory<TourCategory> _repository;

    public GetTourCategoriesRequestHandler(IRepositoryFactory<TourCategory> repository,
        IStringLocalizer<GetTourCategoriesRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TourCategoryDto>> Handle(GetTourCategoriesRequest request, CancellationToken cancellationToken)
    {
        var categories = await _repository.ListAsync(cancellationToken);

        return categories.Adapt<IList<TourCategoryDto>>();
        
        // return request.ParentOnly
        //     ? tourCategories.Adapt<IList<TourCategoryDto>>()
        //         .Where(x => x.TopLevelCategory == true)
        //         .OrderBy(x => x.Name)
        //     : tourCategories.Adapt<IList<TourCategoryDto>>()
        //         .Where(x => !x.TopLevelCategory.HasValue || !x.TopLevelCategory.Value)
        //         .OrderBy(x => x.Name);
    }
}