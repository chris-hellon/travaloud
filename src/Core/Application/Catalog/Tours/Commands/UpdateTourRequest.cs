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
    public string? BookingConfirmationEmailDetails { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? CancellationPolicy { get; set; }
    
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
    
    [Display(Name = "Pick Up Locations")]
    public IEnumerable<TourPickupLocationRequest>? TourPickupLocations { get; set; }

    public IList<TourCategoryLookupRequest>? TourCategoryLookups { get; set; }
    public IList<TourCategoryRequest>? TourCategories { get; set; }
    public IList<TourCategoryRequest>? ParentTourCategories { get; set; }
    public IList<TourImageRequest>? Images { get; set; }
    public IEnumerable<TourDestinationLookupRequest>? TourDestinationLookups { get; set; }
    
    public bool? PublishToSite { get; set; }
    public bool? AdditionalGuestDetailsRequired { get; set; }
}

public class UpdateTourRequestHandler : IRequestHandler<UpdateTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IStringLocalizer<UpdateTourRequestHandler> _localizer;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;
    
    public UpdateTourRequestHandler(IRepositoryFactory<Tour> repository,
        IStringLocalizer<UpdateTourRequestHandler> localizer,
        IFileStorageService file,
        IRepositoryFactory<Page> pageRepository, ICurrentUser currentUser)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _pageRepository = pageRepository;
        _currentUser = currentUser;
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


        var userId = _currentUser.GetUserId();
        
        var updatedTour = tour.Update(request.Name,
            request.Description,
            request.ShortDescription,
            request.Price,
            request.PriceLabel,
            tourImagePath,
            tourImagePath,
            request.MaxCapacity ?? 99999,
            request.MinCapacity,
            request.DayDuration,
            request.NightDuration,
            request.HourDuration,
            request.Address,
            request.TelephoneNumber,
            request.WhatsIncluded,
            request.WhatsNotIncluded,
            request.AdditionalInformation,
            request.MetaKeywords,
            request.MetaDescription,
            request.ImportantInformation,
            request.PublishToSite,
            request.UrlSlug,
            request.H1,
            request.H2,
            tourVideoPath,
            tourMobileVideoPath,
            request.BookingConfirmationEmailDetails,
            request.TermsAndConditions,
            request.CancellationPolicy,
            request.AdditionalGuestDetailsRequired);

        updatedTour.ProcessTourPricesAndDates(request.TourPrices, request.TourDates, request.MaxCapacity ?? 99999, tour.MaxCapacity, userId);
        
        await updatedTour.ProcessImages(request.Images, userId, _file, cancellationToken);
        
        await updatedTour.ProcessTourItineraries(request.TourItineraries, userId, _file, cancellationToken);
        
        updatedTour.ProcessTourCategories(request.TourCategoryId, request.SelectedParentTourCategories, request.TourCategoryLookups, userId);
        updatedTour.ProcessTourDestinations(request.TourDestinationLookups, userId);
        updatedTour.ProcessTourPickupLocations(request.TourPickupLocations, userId);
        
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
    
}