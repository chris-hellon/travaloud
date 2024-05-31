using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourRequest : IRequest<TourDetailsDto>
{
    public GetTourRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetTourRequestHandler : IRequestHandler<GetTourRequest, TourDetailsDto>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<TourDate> _tourDatesRepository;
    private readonly IRepositoryFactory<TourCategory> _tourCategoriesRepository;
    private readonly IRepositoryFactory<TourCategoryLookup> _tourCategoryLookupsRepository;
    private readonly IStringLocalizer<GetTourRequestHandler> _localizer;

    public GetTourRequestHandler(IRepositoryFactory<Tour> repository,
        IRepositoryFactory<TourCategory> tourCategoriesRepository,
        IRepositoryFactory<TourCategoryLookup> tourCategoryLookupsRepository,
        IStringLocalizer<GetTourRequestHandler> localizer, 
        IRepositoryFactory<TourDate> tourDatesRepository)
    {
        _repository = repository;
        _tourCategoriesRepository = tourCategoriesRepository;
        _tourCategoryLookupsRepository = tourCategoryLookupsRepository;
        _localizer = localizer;
        _tourDatesRepository = tourDatesRepository;
    }

    public async Task<TourDetailsDto> Handle(GetTourRequest request, CancellationToken cancellationToken)
    {
        var tourWithoutDates = await _repository.FirstOrDefaultAsync(new TourByIdWithDetailsSpec(request.Id), cancellationToken) 
                   ?? throw new NotFoundException(string.Format(_localizer["tour.notfound"], request.Id));

        var tourDates =
            await _tourDatesRepository.ListAsync(new SearchTourDatesSpec(new SearchTourDatesRequest()
            {
                TourId = request.Id
            }), cancellationToken);

        var tour = tourWithoutDates.Adapt<TourDetailsDto>();
        
        tour.TourDates = tourDates;
        
        if (tour.TourDates != null && tour.TourDates.Any())
            tour.TourDates = tour.TourDates.Where(x => x.StartDate > DateTime.Now).ToList();
        
        if (tour.TourItineraries != null)
        {
            tour.TourItineraries = tour.TourItineraries.OrderBy(x => x.SortOrder ?? int.MaxValue).ToList();

            foreach (var itinerary in tour.TourItineraries)
            {
                var tourItinerarySectionDtos = itinerary.Sections as TourItinerarySectionDto[] ?? itinerary.Sections.ToArray();
                itinerary.Sections = tourItinerarySectionDtos.OrderBy(x => x.SortOrder ?? int.MaxValue).ToList();

                foreach (var section in tourItinerarySectionDtos)
                {
                    section.Images = section.Images.OrderBy(x => x.SortOrder ?? int.MaxValue).ToList();
                }
            }
        }

        var tourCategories = await _tourCategoriesRepository.ListAsync(cancellationToken);

        if (tourCategories.Count <= 0) return tour;
        {
            var parsedTourCategories = tourCategories.Adapt<IList<TourCategoryDto>>();
            var tourCategoryDtos = parsedTourCategories as TourCategoryDto[] ?? parsedTourCategories.ToArray();
            tour.ParentTourCategories = tourCategoryDtos.Where(x => x.TopLevelCategory == true).OrderBy(x => x.Name).ToList();
            tour.TourCategories = tourCategoryDtos.Where(x => !x.TopLevelCategory.HasValue || !x.TopLevelCategory.Value).OrderBy(x => x.Name).ToList();
            // var lookups = await _tourCategoryLookupsRepository.ListAsync(new TourCategoryLookupsByTourCategoryIdsSpec(tourCategoryDtos.Select(x => x.Id), tour.Id), cancellationToken);
            //
            // if (lookups.Count != 0 != true) return tour;
            // {
            //     foreach (var lookup in lookups.Where(lookup => tour.TourCategories.Any(x => x.Id == lookup.TourCategoryId)))
            //     {
            //         tour.TourCategoryId = lookup.TourCategoryId;
            //         break;
            //     }
            //
            //     foreach (var lookup in lookups)
            //     {
            //         tour.SelectedParentTourCategories ??= [];
            //
            //         if (tour.ParentTourCategories.All(x => x.Id != lookup.TourCategoryId)) continue;
            //         {
            //             tour.TourCategoryLookups?.Add(lookup);
            //
            //             var parentTourCategory = tour.ParentTourCategories.First(x => x.Id == lookup.TourCategoryId);
            //
            //             tour.SelectedParentTourCategories?.Add(parentTourCategory.Id);
            //             tour.SelectedParentTourCategoriesString += $"{parentTourCategory.Name}, ";
            //         }
            //     }
            //
            //     if (tour.SelectedParentTourCategoriesString?.Length > 0)
            //     {
            //         tour.SelectedParentTourCategoriesString = tour.SelectedParentTourCategoriesString.Trim().TrimEnd(',');
            //     }
            // }
        }

        return tour;
    }
}