using System.ComponentModel.DataAnnotations;
using Travaloud.Application.Identity.Users;
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

    [Display(Name = "Min Capacity")] public int? MaxCapacity { get; set; }

    [Display(Name = "Max Capacity")] public int? MinCapacity { get; set; }

    [RequiredIfNull("DayDuration", "NightDuration", "HourDuration", ErrorMessage = "A Day, Night or Hour Duration is required")]
    [Display(Name = "Day Duration")]
    public string? DayDuration { get; set; }

    [Display(Name = "Night Duration")] public string? NightDuration { get; set; }

    [Display(Name = "Hour Duration")] public string? HourDuration { get; set; }

    public string? Address { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? WhatsIncluded { get; set; }
    public string? WhatsNotIncluded { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? ImportantInformation { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? BookingConfirmationEmailDetails { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? SupplierEmailText { get; set; }
    public string? SupplierId { get; set; }
    public bool? ShowBookingQRCode { get; set; }
    
    [Display(Name = "Url Slug")] public string? UrlSlug { get; set; }

    [Display(Name = "H1 Tag")] public string? H1 { get; set; }

    [Display(Name = "H2 Tag")] public string? H2 { get; set; }
    
    public string? CustomSeoScripts { get; set; }
    public string? SeoPageTitle { get; set; }
    
    public FileUploadRequest? Image { get; set; }
    public FileUploadRequest? Video { get; set; }
    public FileUploadRequest? MobileVideo { get; set; }
    public DefaultIdType? TourCategoryId { get; set; }
    [Display(Name = "Pricing")] public IList<TourPriceRequest>? TourPrices { get; set; }

    [Display(Name = "Dates")] public IList<TourDateRequest>? TourDates { get; set; }

    [Display(Name = "Itineraries")] public IList<TourItineraryRequest>? TourItineraries { get; set; }

    [Display(Name = "Pick Up Locations")]
    public IEnumerable<TourPickupLocationRequest>? TourPickupLocations { get; set; }
    
    
    public string? SelectedParentTourCategoriesString { get; set; }
    public List<DefaultIdType>? SelectedParentTourCategories { get; set; }
    public IEnumerable<TourPickupLocationRequest>? SelectedPickupLocations = [];
    public IList<TourCategoryRequest>? ParentTourCategories { get; set; }
    public IEnumerable<TourDestinationLookupRequest>? TourDestinationLookups { get; set; }
    public IList<TourImageRequest>? Images { get; set; }
    public IEnumerable<TourDestinationLookupRequest>? SelectedDestinations = [];
    public TourCategoryRequest? TourCategory { get; set; }
    
    public bool? PublishToSite { get; set; }
    public bool? AdditionalGuestDetailsRequired { get; set; }
    public bool? WaiverRequired { get; set; }
}

public class CreateTourRequestHandler : IRequestHandler<CreateTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IRepositoryFactory<TourCategory> _tourCategoryRepository;
    
    public CreateTourRequestHandler(
        IRepositoryFactory<Tour> repository, 
        IFileStorageService file,
        IRepositoryFactory<Page> pageRepository, 
        ICurrentUser currentUser, 
        IUserService userService, IRepositoryFactory<TourCategory> tourCategoryRepository)
    {
        _repository = repository;
        _file = file;
        _pageRepository = pageRepository;
        _currentUser = currentUser;
        _userService = userService;
        _tourCategoryRepository = tourCategoryRepository;
    }

    public async Task<DefaultIdType> Handle(CreateTourRequest request, CancellationToken cancellationToken)
    {
        var tourImagePath = request.Image != null
            ? await _file.UploadAsync<Tour>(request.Image,
                FileType.Image,
                cancellationToken)
            : string.Empty;
        
        var videoPath = request.Video != null
            ? await _file.UploadAsync<Tour>(request.Video, FileType.Video, cancellationToken)
            : string.Empty;
        
        var mobileVideoPath = request.MobileVideo != null
            ? await _file.UploadAsync<Tour>(request.MobileVideo, FileType.Video, cancellationToken)
            : string.Empty;

        var userId = _currentUser.GetUserId();

        var tour = new Tour(request.Name,
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
            videoPath,
            mobileVideoPath,
            request.BookingConfirmationEmailDetails,
            request.TermsAndConditions,
            request.CancellationPolicy,
            request.AdditionalGuestDetailsRequired,
            request.WaiverRequired,
            request.SupplierId,
            request.SupplierEmailText,
            request.TourCategory?.Id,
            request.ShowBookingQRCode,
            request.CustomSeoScripts,
            request.SeoPageTitle);

        // var tourCategory = await _tourCategoryRepository.GetByIdAsync(request.TourCategory?.Id, cancellationToken);
        // tour.TourCategory = tourCategory;
        
        tour.ProcessTourPricesAndDates(request.TourPrices, request.TourDates, request.MaxCapacity ?? 99999,
            tour.MaxCapacity, userId);
        // tour.ProcessTourCategories(request.TourCategoryId, request.SelectedParentTourCategories,
        //     request.TourCategoryLookups, userId);
        tour.ProcessTourDestinations(request.SelectedDestinations, userId);
        tour.ProcessTourPickupLocations(request.SelectedPickupLocations, userId);

        await tour.ProcessTourItineraries(request.TourItineraries, userId, _file, cancellationToken);
        await tour.ProcessImages(request.Images, userId, _file, cancellationToken);
        
        // Add Domain Events to be raised after the commit
        tour.DomainEvents.Add(EntityCreatedEvent.WithEntity(tour));

        await _repository.AddAsync(tour, cancellationToken);

        if (!string.IsNullOrEmpty(request.SupplierId))
        {
            await _userService.CreateClaimAsync(new CreateUserClaimRequest()
            {
                UserId = request.SupplierId,
                ClaimType = "SupplierTour",
                ClaimValue = tour.Id.ToString()
            }, request.SupplierId);
        }
        
        var page = new Page($"Tours - {request.Name}",
            request.MetaKeywords,
            request.MetaDescription,
            tourImagePath,
            string.Empty,
            request.UrlSlug,
            request.H1,
            request.H2, null, request.SeoPageTitle);

        page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));

        await _pageRepository.AddAsync(page, cancellationToken);

        return tour.Id;
    }
}