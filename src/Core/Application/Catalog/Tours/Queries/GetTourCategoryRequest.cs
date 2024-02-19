using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourCategoryRequest : IRequest<TourCategoryDto>
{
    public GetTourCategoryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetTourCategoryRequestHandler : IRequestHandler<GetTourCategoryRequest, TourCategoryDto>
{
    private readonly IRepositoryFactory<TourCategoryLookup> _tourCategoryLookupsRepository;
    private readonly IRepositoryFactory<TourCategory> _repository;
    private readonly IStringLocalizer<GetTourCategoryRequestHandler> _localizer;

    public GetTourCategoryRequestHandler(IRepositoryFactory<TourCategory> repository,
        IRepositoryFactory<TourCategoryLookup> tourCategoryLookupsRepository,
        IStringLocalizer<GetTourCategoryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
        _tourCategoryLookupsRepository = tourCategoryLookupsRepository;
    }

    public async Task<TourCategoryDto> Handle(GetTourCategoryRequest request, CancellationToken cancellationToken)
    {
        var tourCategory =  await _repository.FirstOrDefaultAsync(
                                new TourCategoryByIdSpec(request.Id), cancellationToken)
                            ?? throw new NotFoundException(string.Format(_localizer["tourCategory.notfound"], request.Id));

        var tourCategories = await _repository.ListAsync(cancellationToken);

        if (tourCategories.Count <= 0) return tourCategory;
        var parsedTourCategories = tourCategories.Adapt<IList<TourCategoryDto>>();
        tourCategory.ParentTourCategories =
            parsedTourCategories.Where(x => x.TopLevelCategory == true).OrderBy(x => x.Name).ToList();

        return tourCategory;
    }
}