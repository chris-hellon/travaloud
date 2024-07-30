using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Identity.Users.Password;
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
    public bool? PublishToSite { get; set; }
    public bool? AdditionalGuestDetailsRequired { get; set; }
    public bool? WaiverRequired { get; set; }
    public string? SupplierEmailText { get; set; }
    public string? SupplierId { get; set; }
    
    [Display(Name = "Url Slug")]
    public string? UrlSlug { get; set; }

    [Display(Name = "H1 Tag")]
    public string? H1 { get; set; }

    [Display(Name = "H2 Tag")]
    public string? H2 { get; set; }
    
    public FileUploadRequest? Image { get; set; }
    public FileUploadRequest? Video { get; set; }
    public FileUploadRequest? MobileVideo { get; set; }
    public TourCategoryRequest? TourCategory { get; set; }
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
    public IEnumerable<TourPickupLocationRequest>? SelectedPickupLocations = [];
    public IEnumerable<TourDestinationLookupRequest>? SelectedDestinations = [];
}

public class UpdateTourRequestHandler : IRequestHandler<UpdateTourRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IRepositoryFactory<TourCategory> _tourCategoryRepositor;
    private readonly IRepositoryFactory<TourDestinationLookup> _tourDestinationLookupsRepository;
    private readonly IStringLocalizer<UpdateTourRequestHandler> _localizer;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    
    public UpdateTourRequestHandler(IRepositoryFactory<Tour> repository,
        IStringLocalizer<UpdateTourRequestHandler> localizer,
        IFileStorageService file,
        IRepositoryFactory<Page> pageRepository, 
        ICurrentUser currentUser, 
        IRepositoryFactory<TourDestinationLookup> tourDestinationLookupsRepository, 
        IUserService userService, IRepositoryFactory<TourCategory> tourCategoryRepositor)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _pageRepository = pageRepository;
        _currentUser = currentUser;
        _tourDestinationLookupsRepository = tourDestinationLookupsRepository;
        _userService = userService;
        _tourCategoryRepositor = tourCategoryRepositor;
    }

    public async Task<DefaultIdType> Handle(UpdateTourRequest request, CancellationToken cancellationToken)
    {
        var tour = await _repository.SingleOrDefaultAsync(new TourByIdSpec(request.Id), cancellationToken);

        _ = tour ?? throw new NotFoundException(string.Format(_localizer["tour.notfound"], request.Id));

        var tourDestinationLookups = await _tourDestinationLookupsRepository.ListAsync(new TourDestinationLookupsByTourIdSpec(request.Id), cancellationToken);

        tour.TourDestinationLookups = tourDestinationLookups;
        
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
        
        if (tour.SupplierId.IsNullOrEmpty() && !request.SupplierId.IsNullOrEmpty())
        {
            await _userService.CreateClaimAsync(new CreateUserClaimRequest()
            {
                UserId = request.SupplierId,
                ClaimType = "SupplierTour",
                ClaimValue = tour.Id.ToString()
            }, request.SupplierId);
        }
        else
        {
            if (!request.SupplierId.IsNullOrEmpty() && request.SupplierId != tour.SupplierId)
            {
                await _userService.DeleteClaimAsync(new DeleteUserClaimRequest()
                {
                    UserId = tour.SupplierId,
                    ClaimType = "SupplierTour",
                    ClaimValue = tour.Id.ToString()
                }, tour.SupplierId);
                
                await _userService.CreateClaimAsync(new CreateUserClaimRequest()
                {
                    UserId = request.SupplierId,
                    ClaimType = "SupplierTour",
                    ClaimValue = tour.Id.ToString()
                }, request.SupplierId);
            }
            else if (request.SupplierId.IsNullOrEmpty())
            {
                await _userService.DeleteClaimAsync(new DeleteUserClaimRequest()
                {
                    UserId = tour.SupplierId,
                    ClaimType = "SupplierTour",
                    ClaimValue = tour.Id.ToString()
                }, tour.SupplierId);
            }
        }
        
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
            request.AdditionalGuestDetailsRequired,
            request.WaiverRequired,
            request.SupplierId,
            request.SupplierEmailText,
            request.TourCategory?.Id);

        if (request.TourCategory?.Id != updatedTour.TourCategory?.Id)
        {
            var tourCategory = await _tourCategoryRepositor.GetByIdAsync(request.TourCategory?.Id, cancellationToken);
            updatedTour.TourCategory = tourCategory;
        }
        else updatedTour.ProcessTourCategories(request.SelectedParentTourCategories, userId);
        
        updatedTour.ProcessTourPricesAndDates(request.TourPrices, request.TourDates, request.MaxCapacity ?? 99999, tour.MaxCapacity, userId);
        
        await updatedTour.ProcessImages(request.Images, userId, _file, cancellationToken);
        
        await updatedTour.ProcessTourItineraries(request.TourItineraries, userId, _file, cancellationToken);
        
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