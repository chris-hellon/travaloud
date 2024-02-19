using System.ComponentModel.DataAnnotations;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class UpdateTourRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
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
    public bool DeleteCurrentImage { get; set; }
    public bool DeleteCurrentVideo { get; set; }
    public bool DeleteCurrentMobileVideo { get; set; }
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
    public IList<UpdateTourImageRequest>? Images { get; set; }

    public bool? PublishToSite { get; set; }
}

public class UpdateTourRequestHandler : IRequestHandler<UpdateTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IStringLocalizer<UpdateTourRequestHandler> _localizer;
    private readonly IFileStorageService _file;
    private readonly IRepositoryFactory<TourImage> _tourImageRepository;

    public UpdateTourRequestHandler(IRepositoryFactory<Tour> repository,
        IStringLocalizer<UpdateTourRequestHandler> localizer,
        IFileStorageService file,
        IRepositoryFactory<TourImage> tourImageRepository, IRepositoryFactory<Page> pageRepository)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _tourImageRepository = tourImageRepository;
        _pageRepository = pageRepository;
    }

    public async Task<DefaultIdType> Handle(UpdateTourRequest request, CancellationToken cancellationToken)
    {
        var tour = await _repository.SingleOrDefaultAsync(new TourByIdSpec(request.Id), cancellationToken);

        _ = tour ?? throw new NotFoundException(string.Format(_localizer["tour.notfound"], request.Id));

        var page = await _pageRepository.SingleOrDefaultAsync(new PageByTitleSpec($"Tours - {tour.Name}"), cancellationToken);
        
        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = tour.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            tour = tour.ClearImagePath();
        }

        if (request.DeleteCurrentVideo)
        {
            var currentProductVideoPath = tour.VideoPath;
            if (!string.IsNullOrEmpty(currentProductVideoPath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductVideoPath));
            }

            tour = tour.ClearVideoPath();
        }

        if (request.DeleteCurrentMobileVideo)
        {
            var currentProductMobileVideoPathPath = tour.MobileVideoPath;
            if (!string.IsNullOrEmpty(currentProductMobileVideoPathPath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductMobileVideoPathPath));
            }

            tour = tour.ClearMobileVideoPath();
        }

        var tourImagePath = request.Image is not null
            ? await _file.UploadAsync<Tour>(request.Image, FileType.Image, cancellationToken)
            : null;

        var tourVideoPath = request.Video is not null
            ? await _file.UploadAsync<Tour>(request.Video, FileType.Video, cancellationToken)
            : null;

        var tourMobileVideoPath = request.MobileVideo is not null
            ? await _file.UploadAsync<Tour>(request.MobileVideo, FileType.Video, cancellationToken)
            : null;


        var tourPricesAndDates = UpdateTourPricesAndDates(request, tour, request.MaxCapacity ?? 99999, tour.MaxCapacity);
        var tourItineraries = await UpdateTourItineraries(request, tour, cancellationToken);
        var tourCategories = UpdateTourCategories(request, tour);

        var updatedTour = tour.Update(request.Name, request.Description, request.ShortDescription, request.Price, request.PriceLabel, tourImagePath, tourImagePath, request.MaxCapacity ?? 99999, request.MinCapacity, request.DayDuration, request.NightDuration, request.HourDuration, request.Address, request.TelephoneNumber, request.WhatsIncluded, request.WhatsNotIncluded, request.AdditionalInformation, request.MetaKeywords, request.MetaDescription, request.ImportantInformation, tourPricesAndDates.Item2, tourItineraries, tourPricesAndDates.Item1, tourCategories, null, request.PublishToSite, request.UrlSlug, request.H1, request.H2, tourVideoPath, tourMobileVideoPath);

        await AddImages(request, updatedTour, cancellationToken);
        
        // Add Domain Events to be raised after the commit
        tour.DomainEvents.Add(EntityUpdatedEvent.WithEntity(tour));

        await _repository.UpdateAsync(updatedTour, cancellationToken);

        if (page != null)
        {
            var updatedPage = page.Update($"Tours - {request.Name}", request.MetaKeywords, request.MetaDescription, tourImagePath);
            
            page.DomainEvents.Add(EntityUpdatedEvent.WithEntity(page));
            
            await _pageRepository.UpdateAsync(updatedPage, cancellationToken);
        }
        else
        { 
            page = new Page($"Tours - {request.Name}", request.MetaKeywords, request.MetaDescription, tourImagePath);
        
            page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));
        
            await _pageRepository.AddAsync(page, cancellationToken);
        }
        
        return request.Id;
    }

    private static Tuple<List<TourPrice>, List<TourDate>> UpdateTourPricesAndDates(UpdateTourRequest request, Tour tour, int availableSpaces, int? previousAvailableSpaces)
    {
        List<TourPrice> newTourPrices = new();
        List<TourDate> newTourDates = new();

        if (request.TourPrices?.Any() == true)
        {
            foreach (var tourPrice in request.TourPrices)
            {
                var price = tour.TourPrices?.FirstOrDefault(tp => tp.Id == tourPrice.Id);

                if (price is null)
                {
                    // Create a new TourPrice
                    price = new TourPrice(tourPrice.Price ?? 0m, tourPrice.Title, tourPrice.Description, tourPrice.MonthFrom, tourPrice.MonthTo, tourPrice.DayDuration, tourPrice.NightDuration, tourPrice.HourDuration, request.Id);
                }
                else
                {
                    // Update an existing TourPrice
                    price.Update(tourPrice.Price, tourPrice.Title, tourPrice.Description, tourPrice.MonthFrom, tourPrice.MonthTo, tourPrice.DayDuration, tourPrice.NightDuration, tourPrice.HourDuration, request.Id);
                }

                newTourPrices.Add(price);

                if (request.TourDates?.Any() == true)
                {
                    var priceDates = request.TourDates.Where(x => x.TourPriceId == tourPrice.Id);

                    if (priceDates.Any())
                    {
                        foreach (var tourDateRequest in priceDates)
                        {
                            var date = tour.TourDates?.FirstOrDefault(td => td.Id == tourDateRequest.Id);

                            if (tourDateRequest.StartDate.HasValue && tourDateRequest.EndDate.HasValue && tourDateRequest.StartTime.HasValue)
                            {
                                tourDateRequest.StartDate = tourDateRequest.StartDate.Value.Add(tourDateRequest.StartTime.Value);

                                if (date is null)
                                {
                                    tourDateRequest.AvailableSpaces = availableSpaces;

                                    if (tourDateRequest.AvailableSpaces.HasValue)
                                    {
                                        date = new TourDate(tourDateRequest.StartDate.Value, tourDateRequest.EndDate.Value, tourDateRequest.AvailableSpaces.Value, tourDateRequest.PriceOverride, request.Id, tourDateRequest.TourPriceId, price);
                                    }
                                }
                                else
                                {
                                    tourDateRequest.AvailableSpaces = date.AvailableSpaces;

                                    if (previousAvailableSpaces.HasValue && previousAvailableSpaces.Value != availableSpaces)
                                    {
                                        var dateAvailableSpaces = date.AvailableSpaces;
                                        var previousAvailableDateSpacesDifference = previousAvailableSpaces.Value - dateAvailableSpaces;

                                        if (previousAvailableDateSpacesDifference == 0)
                                        {
                                            tourDateRequest.AvailableSpaces = availableSpaces;
                                        }
                                        else
                                        {
                                            tourDateRequest.AvailableSpaces = availableSpaces - previousAvailableDateSpacesDifference;
                                        }
                                    }
                                    else
                                    {
                                        tourDateRequest.AvailableSpaces = availableSpaces;
                                    }

                                    date.Update(tourDateRequest.StartDate, tourDateRequest.EndDate, tourDateRequest.AvailableSpaces, tourDateRequest.PriceOverride, request.Id, tourDateRequest.TourPriceId);
                                }

                                if (date is not null)
                                {
                                    newTourDates.Add(date);
                                }
                            }
                        }
                    }
                }
            }
        }

        return new Tuple<List<TourPrice>, List<TourDate>>(newTourPrices, newTourDates);
    }

    public async Task<List<TourItinerary>> UpdateTourItineraries(UpdateTourRequest request, Tour tour, CancellationToken cancellationToken)
    {
        List<TourItinerary> tourItineraries = new();

        foreach (var tourItinerary in request.TourItineraries ?? Enumerable.Empty<TourItineraryRequest>())
        {
            var existingTourItinerary = tour.TourItineraries?.FirstOrDefault(td => td.Id == tourItinerary.Id);

            if (existingTourItinerary is null)
            {
                List<TourItinerarySection> itinerarySections = new();

                if (tourItinerary.Sections?.Any() == true)
                {
                    foreach (var section in tourItinerary.Sections)
                    {
                        List<TourItinerarySectionImage> sectionImages = new();

                        if (section.Images?.Any() == true)
                        {
                            foreach (var image in section.Images)
                            {
                                if (!string.IsNullOrEmpty(image.ImageInBytes))
                                {
                                    image.Image = new FileUploadRequest() { Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty, Name = $"{section.Title}_{DefaultIdType.NewGuid():N}" };
                                    var imagePath = await _file.UploadAsync<TourItinerarySectionImageRequest>(image.Image, FileType.Image, cancellationToken);

                                    sectionImages.Add(new TourItinerarySectionImage(imagePath, imagePath));
                                }
                                else
                                {
                                    sectionImages.Add(new TourItinerarySectionImage(image.ImagePath, image.ThumbnailImagePath));
                                }
                            }
                        }

                        itinerarySections.Add(new TourItinerarySection(section.Title, section.SubTitle, section.Description, section.Highlights, sectionImages));
                    }
                }

                tourItineraries.Add(new TourItinerary(tourItinerary.Header, tourItinerary.Title, tourItinerary.Description, itinerarySections));
            }
            else
            {
                // Update an existing TourItinerary
                List<TourItinerarySection> itinerarySections = new();

                if (tourItinerary.Sections?.Any() == true)
                {
                    foreach (var section in tourItinerary.Sections)
                    {
                        List<TourItinerarySectionImage> sectionImages = new();

                        if (section.Images?.Any() == true)
                        {
                            foreach (var image in section.Images)
                            {
                                if (!string.IsNullOrEmpty(image.ImageInBytes))
                                {
                                    image.Image = new FileUploadRequest() { Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty, Name = $"{section.Title}_{DefaultIdType.NewGuid():N}" };
                                    var imagePath = await _file.UploadAsync<TourItinerarySectionImageRequest>(image.Image, FileType.Image, cancellationToken);

                                    sectionImages.Add(new TourItinerarySectionImage(imagePath, imagePath));
                                }
                                else
                                {
                                    sectionImages.Add(new TourItinerarySectionImage(image.ImagePath, image.ThumbnailImagePath));
                                }
                            }
                        }

                        itinerarySections.Add(new TourItinerarySection(section.Title, section.SubTitle, section.Description, section.Highlights, sectionImages));
                    }
                }

                existingTourItinerary.Update(tourItinerary.Header, tourItinerary.Title, tourItinerary.Description, request.Id, itinerarySections);
                tourItineraries.Add(existingTourItinerary);
            }
        }

        return tourItineraries;
    }

    private static List<TourCategoryLookup> UpdateTourCategories(UpdateTourRequest request, Tour tour)
    {
        List<TourCategoryLookup> tourCategories = new();

        if (request.SelectedParentTourCategories?.Count > 0)
        {
            foreach (var lookup in request.SelectedParentTourCategories)
            {
                var existingTourCategory = tour.TourCategoryLookups?.FirstOrDefault(td => td.TourCategoryId == lookup);

                if (existingTourCategory is null)
                {
                    // Create a new TourItinerary
                    tourCategories.Add(new TourCategoryLookup(lookup, null, request.Id));
                }
                else
                {
                    // Update an existing TourItinerary
                    existingTourCategory.Update(request.Id, lookup, null);
                    tourCategories.Add(existingTourCategory);
                }
            }
        }
        else if (request.TourCategoryId.HasValue)
        {
            tourCategories.Add(new TourCategoryLookup(request.TourCategoryId.Value, null, request.Id));
        }

        return tourCategories;
    }

    private async Task AddImages(UpdateTourRequest request, Tour tour, CancellationToken cancellationToken)
    {
        if (request.Images?.Any() == true)
        {
            var images = new List<TourImage>();
            foreach (var imageRequest in request.Images)
            {
                var image = tour.Images?.FirstOrDefault(x => x.Id == imageRequest.Id);

                if (image == null)
                {
                    var imagePath = await _file.UploadAsync<TourImage>(imageRequest.Image, FileType.Image, cancellationToken);
                    images.Add(new TourImage(imagePath, imagePath, imageRequest.SortOrder, tour.Id));
                }
                else
                {
                    if (imageRequest.DeleteCurrentImage)
                    {
                        var currentProductImagePath = imageRequest.ImagePath;
                        if (!string.IsNullOrEmpty(currentProductImagePath))
                        {
                            var root = Directory.GetCurrentDirectory();
                            await _file.Remove(Path.Combine(root, currentProductImagePath));
                        }

                        var tourImage = await _tourImageRepository.SingleOrDefaultAsync(new TourImageByIdSpec(imageRequest.Id), cancellationToken);

                        _ = tourImage ?? throw new NotFoundException(string.Format(_localizer["tourImage.notfound"], request.Id));

                        await _tourImageRepository.DeleteAsync(tourImage, cancellationToken);
                    }

                    var imagePath = imageRequest.Image is not null
                        ? image.ImagePath
                        : null;

                    image.Update(imagePath, imagePath, imageRequest.SortOrder, tour.Id);
                    images.Add(image);
                }
            }

            tour.Images = images;
        }
    }
}