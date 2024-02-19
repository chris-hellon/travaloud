using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetParentTourCategoriesRequest : IRequest<IEnumerable<TourCategoryDto>>
{
}

public class GetParentTourCategoriesRequestHandler : IRequestHandler<GetParentTourCategoriesRequest, IEnumerable<TourCategoryDto>>
{
    private readonly IStringLocalizer<GetParentTourCategoriesRequestHandler> _localizer;
    private readonly IRepositoryFactory<TourCategory> _repository;

    public GetParentTourCategoriesRequestHandler(IRepositoryFactory<TourCategory> repository,
        IStringLocalizer<GetParentTourCategoriesRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TourCategoryDto>> Handle(GetParentTourCategoriesRequest request, CancellationToken cancellationToken)
    {
        var tourCategories = await _repository.ListAsync(cancellationToken);
        return tourCategories.Adapt<IList<TourCategoryDto>>().Where(x => x.TopLevelCategory == true).OrderBy(x => x.Name);
    }
}