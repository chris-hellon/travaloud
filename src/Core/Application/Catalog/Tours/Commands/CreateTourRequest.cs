using System.ComponentModel.DataAnnotations;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class CreateTourRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public decimal? Price { get; set; }
    public string? PriceLabel { get; set; }

    [Display(Name = "Min Capacity")]
    public int? MaxCapacity { get; set; }

    [Display(Name = "Max Capacity")]
    public int? MinCapacity { get; set; }

    [RequiredIfNull("DayDuration", "NightDuration", "HourDuration", ErrorMessage = "A Day, Night or Hour Duration is required")]
    [Display(Name = "Day Duration")]
    public string? DayDuration { get; set; }

    [Display(Name = "Night Duration")]
    public string? NightDuration { get; set; }

    [Display(Name = "Hour Duration")]
    public string? HourDuration { get; set; }

    public string? Address { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? WhatsIncluded { get; set; }
    public string? WhatsNotIncluded { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? ImportantInformation { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }

    [Display(Name = "Url Slug")]
    public string? UrlSlug { get; set; }

    [Display(Name = "H1 Tag")]
    public string? H1 { get; set; }

    [Display(Name = "H2 Tag")]
    public string? H2 { get; set; }
    
    public FileUploadRequest? Image { get; set; }
    public FileUploadRequest? Video { get; set; }
    public FileUploadRequest? MobileVideo { get; set; }
    public DefaultIdType? TourCategoryId { get; set; }
    public string? SelectedParentTourCategoriesString { get; set; }
    public List<DefaultIdType>? SelectedParentTourCategories { get; set; }

    [Display(Name = "Pricing")]
    public IList<TourPriceRequest>? TourPrices { get; set; }

    [Display(Name = "Dates")]
    public IList<TourDateRequest>? TourDates { get; set; }

    [Display(Name = "Itineraries")]
    public IList<TourItineraryRequest>? TourItineraries { get; set; }

    public IList<TourCategoryLookupRequest>? TourCategoryLookups { get; set; }
    public IList<TourCategoryRequest>? TourCategories { get; set; }
    public IList<TourCategoryRequest>? ParentTourCategories { get; set; }
    public IList<CreateTourImageRequest>? Images { get; set; }

    public bool? PublishToSite { get; set; }
}

public class CreateTourRequestHandler : IRequestHandler<CreateTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IFileStorageService _file;

    public CreateTourRequestHandler(IRepositoryFactory<Tour> repository, IFileStorageService file, IRepositoryFactory<Page> pageRepository)
    {
        _repository = repository;
        _file = file;
        _pageRepository = pageRepository;
    }

    public async Task<DefaultIdType> Handle(CreateTourRequest request, CancellationToken cancellationToken)
    {
        var tourImagePath = await _file.UploadAsync<Tour>(request.Image, FileType.Image, cancellationToken);
        var videoPath = request.Video != null ? await _file.UploadAsync<Tour>(request.Video, FileType.Video, cancellationToken) : string.Empty;
        var mobileVideoPath = request.MobileVideo != null ? await _file.UploadAsync<Tour>(request.MobileVideo, FileType.Video, cancellationToken) : string.Empty;

        var tourPricesAndDates = GetTourPricesAndDates(request, request.MaxCapacity ?? 99999);
        var tourItineraries = await AddTourItineraries(request, cancellationToken);
        var tourCategories = AddTourCategories(request);

        var tour = new Tour(request.Name, request.Description, request.ShortDescription, request.Price, request.PriceLabel, tourImagePath, tourImagePath, request.MaxCapacity ?? 99999, request.MinCapacity, request.DayDuration, request.NightDuration, request.HourDuration, request.AdditionalInformation, request.TelephoneNumber, request.WhatsIncluded, request.WhatsNotIncluded, request.AdditionalInformation, request.MetaKeywords, request.MetaDescription, request.ImportantInformation, tourPricesAndDates.Item2, tourItineraries, tourPricesAndDates.Item1, tourCategories, null, request.PublishToSite, request.UrlSlug, request.H1, request.H2, videoPath, mobileVideoPath);

        await AddImages(request, tour, cancellationToken);

        // Add Domain Events to be raised after the commit
        tour.DomainEvents.Add(EntityCreatedEvent.WithEntity(tour));

        await _repository.AddAsync(tour, cancellationToken);

        var page = new Page($"Tours - {request.Name}", request.MetaKeywords, request.MetaDescription, tourImagePath);
        
        page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));
        
        await _pageRepository.AddAsync(page, cancellationToken);
        
        return tour.Id;
    }

    private static Tuple<List<TourPrice>, List<TourDate>> GetTourPricesAndDates(CreateTourRequest request, int? availableSpaces)
    {
        var newTourPrices = new List<TourPrice>();
        var newTourDates = new List<TourDate>();

        if (request.TourPrices?.Any() != true || request.TourDates?.Any() != true)
            
            return new Tuple<List<TourPrice>, List<TourDate>>(newTourPrices, newTourDates);
        foreach (var tourPriceRequest in request.TourPrices)
        {
            // Create a new TourPrice
            var price = new TourPrice(tourPriceRequest.Price ?? 0m, tourPriceRequest.Title, tourPriceRequest.Description, tourPriceRequest.MonthFrom, tourPriceRequest.MonthTo, tourPriceRequest.DayDuration, tourPriceRequest.NightDuration, tourPriceRequest.HourDuration, null);
            newTourPrices.Add(price);

            var priceDates = request.TourDates.Where(x => x.TourPriceId == tourPriceRequest.Id);

            if (!priceDates.Any()) continue;
            
            foreach (var tourDateRequest in priceDates)
            {
                tourDateRequest.AvailableSpaces = availableSpaces;

                if (!tourDateRequest.StartDate.HasValue || !tourDateRequest.StartTime.HasValue ||
                    !tourDateRequest.EndDate.HasValue || !tourDateRequest.AvailableSpaces.HasValue) continue;
                
                tourDateRequest.StartDate = tourDateRequest.StartDate.Value.Add(tourDateRequest.StartTime.Value);
                newTourDates.Add(new TourDate(tourDateRequest.StartDate.Value, tourDateRequest.EndDate.Value, tourDateRequest.AvailableSpaces.Value, tourDateRequest.PriceOverride, null, tourDateRequest.TourPriceId, price));
            }
        }

        return new Tuple<List<TourPrice>, List<TourDate>>(newTourPrices, newTourDates);
    }

    private async Task<List<TourItinerary>> AddTourItineraries(CreateTourRequest request, CancellationToken cancellationToken)
    {
        var newItineraries = new List<TourItinerary>();

        if (request.TourItineraries?.Any() != true) return newItineraries;
        
        foreach (var tourItineraryRequest in request.TourItineraries)
        {
            if (string.IsNullOrEmpty(tourItineraryRequest.Header)) continue;
            
            var itinerarySections = new List<TourItinerarySection>();

            if (tourItineraryRequest.Sections?.Any() == true)
            {
                foreach (var section in tourItineraryRequest.Sections)
                {
                    var sectionImages = new List<TourItinerarySectionImage>();

                    if (section.Images?.Any() == true)
                    {
                        foreach (var image in section.Images)
                        {
                            if (string.IsNullOrEmpty(image.ImageInBytes)) continue;
                            image.Image = new FileUploadRequest() { Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty, Name = $"{section.Title}_{DefaultIdType.NewGuid():N}" };
                            var imagePath = await _file.UploadAsync<TourItinerarySectionImageRequest>(image.Image, FileType.Image, cancellationToken);

                            sectionImages.Add(new TourItinerarySectionImage(imagePath, imagePath));
                        }
                    }

                    itinerarySections.Add(new TourItinerarySection(section.Title, section.SubTitle, section.Description, section.Highlights, sectionImages));
                }
            }

            newItineraries.Add(new TourItinerary(tourItineraryRequest.Header, tourItineraryRequest.Title, tourItineraryRequest.Description, itinerarySections));
        }

        return newItineraries;
    }

    private static List<TourCategoryLookup> AddTourCategories(CreateTourRequest request)
    {
        var tourCategoryLookups = new List<TourCategoryLookup>();

        if (request.TourCategoryId.HasValue)
        {
            tourCategoryLookups.Add(new TourCategoryLookup(request.TourCategoryId.Value, null));
        }

        if (request.TourCategoryLookups?.Any() == true)
        {
            tourCategoryLookups.AddRange(request.TourCategoryLookups.Select(tourCategoryLookupRequest => new TourCategoryLookup(tourCategoryLookupRequest.TourCategoryId, tourCategoryLookupRequest.ParentTourCategoryId)));
        }
        else if (request.SelectedParentTourCategories != null)
        {
            tourCategoryLookups.AddRange(request.SelectedParentTourCategories.Select(lookup => new TourCategoryLookup(lookup, null)));
        }

        return tourCategoryLookups;
    }

    private async Task AddImages(CreateTourRequest request, Tour tour, CancellationToken cancellationToken)
    {
        if (request.Images?.Any() == true)
        {
            var images = new List<TourImage>();

            foreach (var image in request.Images)
            {
                var roomImagePath = await _file.UploadAsync<TourImage>(image.Image, FileType.Image, cancellationToken);
                images.Add(new TourImage(roomImagePath, roomImagePath, image.SortOrder));
            }

            tour.Images = images;
        }
    }
}