using System.Security.Claims;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Commands;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Basket.Queries;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.PageSorting.Dto;
using Travaloud.Application.Catalog.PageSorting.Queries;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Infrastructure.Mailing;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Multitenancy.TenantWebsite;
using Travaloud.Shared.Authorization;
using Travaloud.Tenants.SharedRCL.Pages.Checkout;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class TravaloudBasePageModel : PageModel
{
    #region Injected Services

    private ILogger? _logger;

    public ILogger Logger => (_logger ??= HttpContext.RequestServices.GetService<ILogger>()) ??
                             throw new InvalidOperationException();

    private IHttpContextAccessor? _httpContextAccessor;

    public IHttpContextAccessor HttpContextAccessor =>
        (_httpContextAccessor ??= HttpContext.RequestServices.GetService<IHttpContextAccessor>()) ??
        throw new InvalidOperationException();

    private ICurrentUser? _currentUser;

    protected ICurrentUser CurrentUser => (_currentUser ??= HttpContext.RequestServices.GetService<ICurrentUser>()) ??
                                          throw new InvalidOperationException();

    private IPropertiesService? _propertiesService;

    protected IPropertiesService PropertiesService =>
        (_propertiesService ??= HttpContext.RequestServices.GetService<IPropertiesService>()) ??
        throw new InvalidOperationException();

    private IBookingsService? _bookingService;

    protected IBookingsService BookingService =>
        (_bookingService ??= HttpContext.RequestServices.GetService<IBookingsService>()) ??
        throw new InvalidOperationException();

    private IBasketService? _basketService;

    protected IBasketService BasketService =>
        (_basketService ??= HttpContext.RequestServices.GetService<IBasketService>()) ??
        throw new InvalidOperationException();

    private IStripeService? _stripeService;

    protected IStripeService StripeService =>
        (_stripeService ??= HttpContext.RequestServices.GetService<IStripeService>()) ??
        throw new InvalidOperationException();

    private ITenantWebsiteService? _tenantWebsiteService;

    protected ITenantWebsiteService TenantWebsiteService =>
        (_tenantWebsiteService ??= HttpContext.RequestServices.GetService<ITenantWebsiteService>()) ??
        throw new InvalidOperationException();

    private IMultiTenantContextAccessor<TravaloudTenantInfo>? _multiTenantContextAccessor;

    protected IMultiTenantContextAccessor<TravaloudTenantInfo>? MultiTenantContextAccessor =>
        _multiTenantContextAccessor ??=
            HttpContext.RequestServices.GetService<IMultiTenantContextAccessor<TravaloudTenantInfo>>();

    private IRazorPartialToStringRenderer? _razorPartialToStringRenderer;

    protected IRazorPartialToStringRenderer RazorPartialToStringRenderer =>
        (_razorPartialToStringRenderer ??= HttpContext.RequestServices.GetService<IRazorPartialToStringRenderer>()) ??
        throw new InvalidOperationException();

    private IMailService? _mailService;

    protected IMailService MailService => (_mailService ??= HttpContext.RequestServices.GetService<IMailService>()) ??
                                          throw new InvalidOperationException();

    private SignInManager<ApplicationUser>? _signInManager;

    protected SignInManager<ApplicationUser> SignInManager =>
        (_signInManager ??= HttpContext.RequestServices.GetService<SignInManager<ApplicationUser>>()) ??
        throw new InvalidOperationException();

    private UserManager<ApplicationUser>? _userManager;

    protected UserManager<ApplicationUser> UserManager =>
        (_userManager ??= HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>()) ??
        throw new InvalidOperationException();

    #endregion

    private TravaloudTenantInfo? _tenantInfo;

    protected TravaloudTenantInfo TenantInfo =>
        (_tenantInfo ??= MultiTenantContextAccessor?.MultiTenantContext?.TenantInfo) ??
        throw new InvalidOperationException();

    //TODO: App configuration > add this into travaloud admin
    private TravaloudSettings? _travaloudSettings;

    public TravaloudSettings TravaloudSettings =>
        (_travaloudSettings ??= HttpContext.RequestServices.GetService<IOptions<TravaloudSettings>>()?.Value) ??
        throw new InvalidOperationException();

    private TravaloudTenantSettings? _tenantSettings;

    public TravaloudTenantSettings TenantSettings =>
        (_tenantSettings ??= TravaloudSettings.Tenant) ?? throw new InvalidOperationException();

    private TravaloudUrlSettings? _urlSettings;

    public TravaloudUrlSettings UrlSettings => (_urlSettings ??= TravaloudSettings.UrlConfiguration) ??
                                               throw new InvalidOperationException();

    private TravaloudMetaDataSettings? _metaDataSettings;

    public TravaloudMetaDataSettings MetaDataSettings => (_metaDataSettings ??= TravaloudSettings.MetaData) ??
                                                         throw new InvalidOperationException();

    private MailSettings? _mailSettings;

    protected MailSettings MailSettings =>
        (_mailSettings ??= HttpContext.RequestServices.GetService<IOptions<MailSettings>>()?.Value) ??
        throw new InvalidOperationException();

    public TravaloudNavigationSettings? NavigationSettings
    {
        get
        {
            var navlinksWithChildEntities =
                TravaloudSettings.NavigationConfiguration?.NavigationLinks.Where(x => x.ChildrenEntity != null);

            if (navlinksWithChildEntities == null) return TravaloudSettings.NavigationConfiguration;
            {
                var navlinkWithChildEntitieses = navlinksWithChildEntities as NavigationLinkModel[] ??
                                                 navlinksWithChildEntities.ToArray();

                if (navlinkWithChildEntitieses.Length == 0) return TravaloudSettings.NavigationConfiguration;
                {
                    foreach (var navlinkWithChildEntities in navlinkWithChildEntitieses)
                    {
                        switch (navlinkWithChildEntities.ChildrenEntity)
                        {
                            case "Properties":
                                if (Properties != null)
                                    navlinkWithChildEntities.Children = Properties.Select(x =>
                                            new NavigationLinkModel(x.Name, "/Property/Index", x.FriendlyUrl,
                                                new Dictionary<string, string>() {{"propertyName", x.FriendlyUrl}}))
                                        .ToArray();
                                break;
                            case "Tours":
                                if (Tours != null)
                                    navlinkWithChildEntities.Children = Tours.Select(x =>
                                        new NavigationLinkModel(x.Name, "/Tour/Index", x.FriendlyUrl,
                                            new Dictionary<string, string>() {{"tourName", x.FriendlyUrl}})).ToArray();
                                break;
                            case "Destinations":
                                if (Destinations != null)
                                    navlinkWithChildEntities.Children = Destinations.Select(x =>
                                            new NavigationLinkModel(x.Name, "/Destination/Index", x.FriendlyUrl,
                                                new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}}))
                                        .ToArray();
                                break;
                            case "ToursWithCategories":
                                navlinkWithChildEntities.Children = GetTourCategoriesNavigation();
                                break;
                            case "Services":
                                if (Services != null)
                                    navlinkWithChildEntities.Children = Services.Select(x =>
                                            new NavigationLinkModel(x.Title, "/Service/Index", x.FriendlyUrl,
                                                new Dictionary<string, string>() {{"serviceName", x.FriendlyUrl}}))
                                        .ToArray();
                                break;
                        }
                    }
                }
            }

            return TravaloudSettings.NavigationConfiguration;
        }
    }

    #region Entities

    /// <summary>
    /// An array of Tenant Properties
    /// </summary>
    public IEnumerable<PropertyDto>? Properties { get; private set; }

    /// <summary>
    /// An array of Tenant Tours
    /// </summary>
    public IEnumerable<TourDto>? Tours { get; private set; }
    
    /// <summary>
    /// An array of Tenant Hidden Tours
    /// </summary>
    public IEnumerable<TourDto>? AllTours { get; private set; }

    /// <summary>
    /// An array of Tenant Destinations
    /// </summary>
    public IEnumerable<DestinationDto>? Destinations { get; private set; }

    /// <summary>
    /// An array of Tenant Services
    /// </summary>
    public IEnumerable<ServiceDto>? Services { get; private set; }

    /// <summary>
    /// An array of Tenant Tours & Tour Categories
    /// </summary>
    public IEnumerable<TourWithCategoryDto>? ToursWithCategories { get; set; }

    /// <summary>
    /// An array of Tenant Tours & Tour Categories
    /// </summary>
    public IEnumerable<TourWithCategoryDto>? DestinationsWithCategories { get; set; }

    /// <summary>
    /// An array of Page Sortings
    /// </summary>
    public IEnumerable<PageSortingDto>? PageSortings { get; set; }

    #endregion

    #region Properties

    /// <summary>
    /// Retrieves a Tenants Id from appsettings.json
    /// </summary>
    private string? _tenantId;

    public string TenantId
    {
        get
        {
            _tenantId ??= TenantInfo.Id;

            if (_tenantId != null)
                return _tenantId;

            throw new Exception("No TenantId provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Name from appsettings.json
    /// </summary>
    private string? _tenantName;

    public string TenantName
    {
        get
        {
            _tenantName ??= TenantInfo.Name;

            if (_tenantName != null)
                return _tenantName;

            throw new Exception("No TenantName provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Tagline from appsettings.json
    /// </summary>
    private string? _tenantTagLine;

    public string TenantTagLine
    {
        get
        {
            _tenantTagLine ??= TenantSettings.TenantTagLine;

            if (_tenantTagLine != null)
                return _tenantTagLine;

            throw new Exception("No TenantTagLine provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Website Url from appsettings.json
    /// </summary>
    private string? _websiteUrl;

    public string WebsiteUrl
    {
        get
        {
            _websiteUrl ??= UrlSettings.WebsiteUrl;

            if (_websiteUrl != null)
                return _websiteUrl;

            throw new Exception("No WebsiteUrl provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Azure Storage Url from appsettings.json
    /// </summary>
    private string? _tenantAzureStorageUrl;

    protected string TenantAzureStorageUrl => _tenantAzureStorageUrl ??= $"https://travaloud.azureedge.net/{TenantId}";

    /// <summary>
    /// Retrieves a Tenants Property Booking Url from appsettings.json, or returns a default
    /// </summary>
    private string? _propertyBookingUrl;

    public string PropertyBookingUrl
    {
        get
        {
            _propertyBookingUrl ??= UrlSettings.PropertyBookingUrl;

            return _propertyBookingUrl;
        }
    }

    /// <summary>
    /// Retrieves a Tenants Property Booking Url from appsettings.json, or returns a default
    /// </summary>
    private string? _tourBookingUrl;

    public string TourBookingUrl
    {
        get
        {
            _tourBookingUrl ??= UrlSettings.TourBookingUrl;

            return _tourBookingUrl;
        }
    }

    /// <summary>
    /// Retrieves a Tenants Account Management Url from appsettings.json, or returns a default
    /// </summary>
    private string? _accountManagementUrl;

    protected string AccountManagementUrl
    {
        get
        {
            _accountManagementUrl ??= UrlSettings.AccountManagementUrl;

            return _accountManagementUrl;
        }
    }

    /// <summary>
    /// Retrieves a Tenants Account Management Url from appsettings.json
    /// </summary>
    private string? _accountManagementImageUrl;

    public string AccountManagementImageUrl => _accountManagementImageUrl ??= UrlSettings.AccountManagementImageUrl;

    /// <summary>
    /// Retrieves a Tenants Google Analytics Tag from appsettings.json
    /// </summary>
    private string? _googleTagManagerKey;

    public string GoogleTagManagerKey
    {
        get
        {
            _googleTagManagerKey ??= TenantSettings.GoogleTagManagerKey;

            if (_googleTagManagerKey != null)
                return _googleTagManagerKey;

            throw new Exception("No GoogleTagManagerKey provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Google Site Verification from appsettings.json
    /// </summary>
    private string? _googleSiteVerificationKey;

    public string GoogleSiteVerificationKey =>
        (_googleSiteVerificationKey ??= TenantSettings.GoogleSiteVerificationKey);

    /// <summary>
    /// Retrieves a Tenants Facebook Page Id from appsettings.json
    /// </summary>
    private string? _facebookPageId;

    public string FacebookPageId
    {
        get
        {
            _facebookPageId ??= TenantSettings.FacebookPageId;

            if (_facebookPageId != null)
                return _facebookPageId;

            throw new Exception("No FacebookPageId provided in appsettings.json.");
        }
    }

    /// <summary>
    /// Retrieves a Tenants Email Address for sending mail from appsettings.json
    /// </summary>
    private string? _emailAddress;

    protected string EmailAddress
    {
        get
        {
            if (_emailAddress != null)
                return _emailAddress;

            _emailAddress = MailSettings.UserName ?? throw new Exception("No Username provided in appsettings.json.");
            return _emailAddress;
        }
    }

    /// <summary>
    /// Retrieves a Tenants Email Display name for sending mail from appsettings.json
    /// </summary>
    private string? _emailDisplayName;

    protected string EmailDisplayName
    {
        get
        {
            if (_emailDisplayName != null)
                return _emailDisplayName;

            _emailDisplayName = MailSettings.DisplayName ??
                                throw new Exception("No DisplayName provided in appsettings.json.");
            return _emailDisplayName;
        }
    }

    /// <summary>
    /// Retrieves a Tenants default Meta Keywords from appsettings.json
    /// </summary>
    public virtual string MetaKeywords()
    {
        if (MetaDataSettings.MetaKeywords != null)
            return MetaDataSettings.MetaKeywords;

        throw new Exception("No MetaKeywords provided in appsettings.json.");
    }

    /// <summary>
    /// Retrieves a Tenants default Meta Description from appsettings.json
    /// </summary>
    public virtual string MetaDescription()
    {
        if (MetaDataSettings.MetaDescription != null)
            return MetaDataSettings.MetaDescription;

        throw new Exception("No MetaDescription provided in appsettings.json.");
    }

    /// <summary>
    /// Retrieves a Tenants default Meta Image Url from appsettings.json
    /// </summary>
    public virtual string MetaImageUrl()
    {
        if (MetaDataSettings.MetaImageUrl != null)
            return MetaDataSettings.MetaImageUrl;

        throw new Exception("No MetaImageUrl provided in appsettings.json.");
    }

    public virtual Guid? PageId()
    {
        return null;
    }

    /// <summary>
    /// Retrieves a Azure Blob Url from Tenant Azure Storage
    /// </summary>
    /// <param name="blobName"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public string GetAzureBlobUrl(string blobName, string? folderName = null)
    {
        return $"{TenantAzureStorageUrl}/assets/{(folderName != null ? $"{folderName}/" : "")}{blobName}";
    }

    /// <summary>
    /// Retrieves a logged in User Id
    /// </summary>
    private Guid? _userId;

    public Guid? UserId
    {
        get
        {
            if (_userId.HasValue)
                return _userId;

            var currentUserId = CurrentUser.GetUserId();
            if (currentUserId == Guid.Empty)
                _userId = null;
            else
                _userId = currentUserId;

            return _userId;
        }
    }

    public List<TourWithCategoryDto> PromotedTours
    {
        get
        {
            var promotedTourIds = PromotedToursIds();

            if (promotedTourIds != null)
                return promotedTourIds.Select(tourId => ToursWithCategories?.FirstOrDefault(x => x.Id == tourId))
                    .OfType<TourWithCategoryDto>().ToList();

            return [];
        }
    }

    public virtual List<Guid>? PromotedToursIds()
    {
        return null;
    }

    [TempData] public string? StatusMessage { get; set; }

    [TempData] public string? StatusSeverity { get; set; }

    #endregion

    /// <summary>
    /// Sets all base data to be used throughout the application.
    /// </summary>
    /// <returns></returns>
    public virtual async Task OnGetDataAsync()
    {
        var cancellationToken = new CancellationToken();
        var propertiesTask = Task.Run(() => TenantWebsiteService.GetProperties(cancellationToken), cancellationToken);
        var toursTask = Task.Run(() => TenantWebsiteService.GetTours(cancellationToken), cancellationToken);
        var servicesTask = Task.Run(() => TenantWebsiteService.GetServices(cancellationToken), cancellationToken);
        var destinationsTask = Task.Run(() => TenantWebsiteService.GetDestinations(cancellationToken), cancellationToken);

        await Task.WhenAll(propertiesTask, toursTask, servicesTask, destinationsTask);
        
        Properties = propertiesTask.Result;
        Tours = toursTask.Result.Where(x => x.PublishToSite.HasValue && x.PublishToSite.Value);
        AllTours = toursTask.Result;
        Services = servicesTask.Result;
        Destinations = destinationsTask.Result;
    
        if (TenantId != "fuse")
        {
            var toursWithCategoriesTask = TenantWebsiteService.GetToursWithCategories(TenantId, cancellationToken);
            var pageSortingsTask = TenantWebsiteService.GetPageSortings(new GetPageSortingsRequest());

            await Task.WhenAll(toursWithCategoriesTask, pageSortingsTask);
            
            ToursWithCategories = toursWithCategoriesTask.Result;
            await SetToursPrices(ToursWithCategories);
            PageSortings = pageSortingsTask.Result;
        }
        
        StatusSeverity ??= "success";

        LoginModal.BookingUrl = PropertyBookingUrl;
    }

    /// <summary>
    /// Creates a Property Booking Url.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IActionResult> OnPostSearchRoomsAsync()
    {
        Guid? userId = CurrentUser.GetUserId();

        if (userId == Guid.Empty)
            userId = null;

        var propertyId = GetFormValue("PropertyId");
        var checkInDate = GetFormValue("CheckInDate");
        var checkOutDate = GetFormValue("CheckOutDate");

        if (propertyId != null && checkInDate != null && checkOutDate != null)
        {
            if (Guid.TryParse(propertyId,
                    out var propertyIdParsed) &&
                DateTime.TryParse(checkInDate,
                    out var checkInDateParsed) &&
                DateTime.TryParse(checkOutDate,
                    out var checkOutDateParsed))
            {
                var property = await PropertiesService.GetAsync(propertyIdParsed);
                var propertyName = property?.Name.UrlFriendly();

                var url =
                    $"/{PropertyBookingUrl}/{propertyName}/{checkInDateParsed.ToString("yyyy-MM-dd").UrlFriendly()}/{checkOutDateParsed.ToString("yyyy-MM-dd").UrlFriendly()}{(userId != null ? $"/{userId}" : "")}";
                return LocalRedirect(url);
            }
        }

        StatusMessage = "Please select a Hostel & Check In / Out date";
        StatusSeverity = "danger";
        return LocalRedirect(HttpContextAccessor.HttpContext?.Request.Path ?? throw new InvalidOperationException());
    }

    /// <summary>
    /// If a Tenant is using Tours & Tour Categories, retrieve them as Navigation Models
    /// </summary>
    /// <returns></returns>
    private NavigationLinkModel[] GetTourCategoriesNavigation()
    {
        var parentTourWithCategories = ToursWithCategories?.Where(x =>
            !x.TourCategoryId.HasValue && !x.ParentTourCategoryId.HasValue && !x.GroupParentCategoryId.HasValue);

        return parentTourWithCategories?.Select(ConvertTourToNavigationLinkModel).ToArray() ??
               Array.Empty<NavigationLinkModel>();
    }

    /// <summary>
    /// Converts a Tour or Tour Category to a Navigation Link model
    /// </summary>
    /// <param name="tour"></param>
    /// <returns></returns>
    public NavigationLinkModel ConvertTourToNavigationLinkModel(TourWithCategoryDto tour)
    {
        var navigatePage = tour.IsCategory ? "/TourCategory/Index" :
            tour.IsDestination ? "/Destination/Index" : "/Tour/Index";
        var navigationLinkModel = new NavigationLinkModel(tour.Name, navigatePage, tour.FriendlyUrl,
            new Dictionary<string, string>()
            {
                {
                    $"{(tour.IsCategory ? "tourCategoryName" : tour.IsDestination ? "destinationName" : "tourName")}",
                    tour.FriendlyUrl
                }
            });

        // Get child tours with matching CategoryId
        var childTours = ToursWithCategories?.Where(t =>
                t.TourCategoryId == tour.Id || t.ParentTourCategoryId == tour.Id || t.GroupParentCategoryId == tour.Id)
            .ToList();

        if (DestinationsWithCategories != null && DestinationsWithCategories.Any())
            childTours?.AddRange(DestinationsWithCategories.Where(t => t.TourCategoryId == tour.Id).ToList());

        // If there are child tours, recursively convert each one to a NavigationLinkModel and add to the Children array
        if (childTours is {Count: 0}) return navigationLinkModel;
        navigationLinkModel.Children = childTours?.Select(ConvertTourToNavigationLinkModel).ToArray() ??
                                       Array.Empty<NavigationLinkModel>();

        return navigationLinkModel;
    }

    /// <summary>
    /// Retrieves a value from a posted form
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private string? GetFormValue(string key)
    {
        string? returnValue = null;
        var parentKeys = new[] {"CarouselComponent", "BookNowBanner", "BookNowComponent"};

        foreach (var parentKey in parentKeys)
        {
            var keyValue = $"{parentKey}.{key}";
            if (Request.Form.ContainsKey($"{parentKey}.{key}"))
                returnValue = Request.Form[keyValue].ToString();
        }

        if (string.IsNullOrEmpty(returnValue) && Request.Form.ContainsKey(key))
            returnValue = Request.Form[key].ToString();

        return returnValue;
    }

    public async Task SetToursPrices(IEnumerable<TourWithCategoryDto> toursWithCategories)
    {
        var tourWithCategoryDtos = toursWithCategories as TourWithCategoryDto[] ?? toursWithCategories.ToArray();
        var tourIds = tourWithCategoryDtos.Select(x => x.Id).Distinct();
        var tourPrices = await TenantWebsiteService.GetTourPrices(new GetTourPricesRequest(tourIds), CancellationToken.None);
        
        toursWithCategories = tourWithCategoryDtos.Select(x =>
        {
            var tour = Tours.FirstOrDefault(t => t.Id == x.Id);

            if (tour != null)
            {
                var prices = tourPrices.Where(x => x.TourId == tour.Id);
                x.TourPrices = prices;
            }
            else
            {
                var categoriesWithinCategory = ToursWithCategories.Where(t =>
                    t.ParentTourCategoryId.HasValue && t.ParentTourCategoryId.Value == x.Id);
                var toursWithinCategory = ToursWithCategories.Where(t => t.TourCategoryId == x.Id);

                if (!toursWithinCategory.Any() && !categoriesWithinCategory.Any()) return x;
                {
                    var mergedTours = new List<TourWithCategoryDto>();
                    mergedTours.AddRange(toursWithinCategory);
                    mergedTours.AddRange(categoriesWithinCategory);

                    x.ChildTours = mergedTours.OrderBy(x => x.Name).ToList();

                    var tourIds = toursWithinCategory.Select(x => x.Id).ToList();

                    if (categoriesWithinCategory.Any())
                    {
                        var categoryIds = categoriesWithinCategory.Select(x => x.Id).ToList();
                        var categoryTours = ToursWithCategories.Where(t =>
                            t.TourCategoryId.HasValue && categoryIds.Contains(t.TourCategoryId.Value));

                        if (categoryTours.Any())
                            tourIds.AddRange(categoryTours.Select(t => t.Id));
                    }
                    
                    var toursPrices = Tours.Where(t => tourIds.Contains(t.Id)).SelectMany(t => tourPrices.Where(x => x.TourId == t.Id)).MinBy(t => t.Price);

                    if (toursPrices != null)
                        x.TourPrices = new List<TourPriceDto>() {toursPrices};
                }
            }

            return x;
        }).ToList();
    }

    public async Task SetPropertyInformation(PropertyDetailsDto property)
    {
        var propertyInformation = await PropertiesService.GetAsync(property.Id);
        property.Rooms = propertyInformation?.Rooms;
        property.Facilities = propertyInformation?.Facilities;
        property.Directions = propertyInformation?.Directions;
        property.Tours = propertyInformation?.Tours;
    }

    #region Page Models

    [BindProperty] public LoginModalComponent LoginModal { get; set; } = new();

    [BindProperty] public RegisterModalComponent RegisterModal { get; set; } = new();

    #endregion

    #region Identity

    // public async Task<IActionResult> OnGetExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null)
    // {
    //     if (remoteError != null)
    //     {
    //         StatusMessage = $"Error from external provider: {remoteError}";
    //         StatusSeverity = "danger;";
    //
    //         return LocalRedirect("/");
    //     }
    //
    //     var info = await SignInManager.GetExternalLoginInfoAsync();
    //     if (info == null)
    //     {
    //         StatusMessage = "Invalid login attempt.";
    //         StatusSeverity = "danger;";
    //
    //         return LocalRedirect("/");
    //     }
    //
    //     var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
    //         isPersistent: false, bypassTwoFactor: true);
    //
    //     var email = info.Principal.FindFirst(ClaimTypes.Email).Value;
    //     var firstName = info.Principal.FindFirst(ClaimTypes.GivenName).Value;
    //     var lastName = info.Principal.FindFirst(ClaimTypes.Surname).Value;
    //     //var gender = info.Principal.FindFirst(ClaimTypes.Gender).Value;
    //     //var dateOfBirth = info.Principal.FindFirst(ClaimTypes.DateOfBirth).Value;
    //
    //     if (info.LoginProvider == "Google")
    //     {
    //         var options = GoogleAuthenticationConfigurationOptions;
    //
    //         if (options != null)
    //         {
    //             var accessToken = await HttpContext.GetTokenAsync("Google", "access_token");
    //
    //             if (accessToken != null)
    //             {
    //                 using var httpClient = HttpClientFactory.CreateClient();
    //                 httpClient.DefaultRequestHeaders.Authorization =
    //                     new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    //
    //                 var response =
    //                     await httpClient.GetAsync(
    //                         "https://people.googleapis.com/v1/people/me?personFields=birthdays,genders");
    //
    //                 if (response.IsSuccessStatusCode)
    //                 {
    //                     var userData = await response.Content.ReadAsStringAsync();
    //                     var userDataParsed = JObject.Parse(userData);
    //
    //                     if (userDataParsed != null)
    //                     {
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //
    //
    //     if (result.Succeeded)
    //     {
    //         var localRedirect = returnUrl == null;
    //
    //         if (returnUrl != null && returnUrl.Contains(PropertyBookingUrl))
    //         {
    //             var user = await UserManager.FindByEmailAsync(email, TenantId);
    //
    //             if (user != null)
    //             {
    //                 var userId = user.Id;
    //
    //                 returnUrl = returnUrl += $"/{userId}";
    //             }
    //         }
    //
    //         returnUrl ??= $"/{AccountManagementUrl}";
    //         return localRedirect ? LocalRedirect(returnUrl) : Redirect(returnUrl);
    //     }
    //
    //     if (result.RequiresTwoFactor)
    //     {
    //         returnUrl = $"/login-with-2fa{(returnUrl != null ? $"?returnUrl={returnUrl}" : "")}";
    //         return LocalRedirect(returnUrl);
    //     }
    //
    //     if (result.IsLockedOut)
    //     {
    //         return RedirectToPage("/lockout");
    //     }
    //     else
    //     {
    //         ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //         StatusMessage = "Invalid login attempt.";
    //         StatusSeverity = "danger;";
    //     }
    //
    //     return LocalRedirect("/");
    // }
    //
    // public IActionResult OnPostExternalSignIn(string provider)
    // {
    //     var redirectUrl = Url.Page("/Home/Index", pageHandler: "ExternalLoginCallback",
    //         values: new {@returnUrl = LoginModal.ReturnUrl});
    //     var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    //     return new ChallengeResult(provider, properties);
    // }

    /// <summary>
    /// Logs in a user
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostSignInAsync()
    {
        var returnUrl = LoginModal.ReturnUrl;

        var user = await UserManager.FindByEmailAsync(LoginModal.Email);

        if (user != null)
        {
            var userId = user.Id;
            var result = await SignInManager.CheckPasswordSignInAsync(user, LoginModal.Password, false);

            if (result.Succeeded)
            {
                if (TenantInfo is {Identifier: not null})
                {
                    //Set user claims of current tenant for Finbuckle MultiTenancy
                    await SignInManager.SignInWithClaimsAsync(user, null, new Claim[]
                    {
                        new(TravaloudClaims.Tenant, TenantInfo.Identifier)
                    });

                    var localRedirect = returnUrl == null;

                    if (returnUrl != null && returnUrl.Contains(PropertyBookingUrl))
                    {
                        returnUrl += $"/{userId}";
                    }

                    returnUrl ??= $"/{AccountManagementUrl}";
                    return localRedirect ? LocalRedirect(returnUrl) : Redirect(returnUrl);
                }
            }

            if (result.RequiresTwoFactor)
            {
                returnUrl = $"/login-with-2fa{(returnUrl != null ? $"?returnUrl={returnUrl}" : "")}";
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("/lockout");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        StatusMessage = "Invalid login attempt.";
        StatusSeverity = "danger;";

        return LocalRedirect("/");
    }

    /// <summary>
    /// Registers a user, assigns the user to a Guest role
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostRegisterAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Email");
            ModelState.Remove("Message");
            ModelState.Remove("Gender");
            ModelState.Remove("Surname");
            ModelState.Remove("Password");
            ModelState.Remove("FirstName");
            ModelState.Remove("Nationality");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Date");
            ModelState.Remove("NumberOfPeople");
            ModelState.Remove("TourName");
            ModelState.Remove("TourDate");
            ModelState.Remove("PropertyId");
            ModelState.Remove("CheckInDate");
            ModelState.Remove("Description");
            ModelState.Remove("FriendlyUrl");
            ModelState.Remove("CheckOutDate");
            ModelState.Remove("GuestQuantity");
            ModelState.Remove("ContactNumber");
        }

        if (!ModelState.IsValid)
        {
            StatusMessage = "<p>Please complete the Google Captcha to send your message.</p>";
            StatusSeverity = "danger";
            return Redirect(Request.GetEncodedUrl());
        }

        try
        {
            var user = new ApplicationUser()
            {
                FirstName = RegisterModal.FirstName,
                LastName = RegisterModal.Surname,
                FullName = $"{RegisterModal.FirstName} {RegisterModal.Surname}",
                PhoneNumber = RegisterModal.PhoneNumber,
                Gender = RegisterModal.Gender,
                Nationality = RegisterModal.Nationality,
                DateOfBirth = RegisterModal.DateOfBirth,
                UserName = RegisterModal.Email,
                Email = RegisterModal.Email,
                SignUpDate = DateTime.Now,
                IsActive = true,
                EmailConfirmed = true,
                RefreshTokenExpiryTime = DateTime.Now
            };

            if (RegisterModal.Password != null)
            {
                //Register user
                var result = await UserManager.CreateAsync(user, RegisterModal.Password);

                if (result.Succeeded)
                {
                    //Assign user to Guest role
                    var userResult = await UserManager.AddToRoleAsync(user, TravaloudRoles.Guest);

                    if (userResult.Succeeded)
                    {
                        //Sign user in
                        await SignInManager.SignInAsync(user, isPersistent: false);

                        Logger.Information("User with email {Email} registered successfully", user.Email);

                        var returnUrl = RegisterModal.ReturnUrl;
                        var localRedirect = returnUrl == null;

                        //If we're on the property booking page, append user id to the url
                        if (returnUrl != null && returnUrl.Contains(PropertyBookingUrl))
                            returnUrl += $"/{user.Id}";

                        returnUrl ??= $"/{AccountManagementUrl}";
                        return localRedirect ? LocalRedirect(returnUrl) : Redirect(returnUrl);
                    }
                }
                else
                {
                    var errorMessage =
                        result.Errors.Aggregate("", (current, error) => current + (error.Description + ". "));

                    StatusMessage = errorMessage;
                    StatusSeverity = "danger;";
                }
            }
        }
        catch (Exception)
        {
            StatusMessage = "An error occured registering you. Please try again or Contact Us for assistance.";
            StatusSeverity = "danger;";
        }

        await OnGetDataAsync();

        return LocalRedirect("/");
    }

    #endregion

    #region Ajax Methods

    /// <summary>
    /// Validates a user for login
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostValidateUser([FromBody] ValidateUserDTO model)
    {
        var message =
            "<p><strong>We were unable to find a user with the details provided.</strong></p><p>Please ensure your details are correct, or Contact Us</p>";

        try
        {
            var user = await UserManager.FindByEmailAsync(model.Username);

            if (user != null)
            {
                var passwordCorrect = await UserManager.CheckPasswordAsync(user, model.Password);

                if (passwordCorrect)
                    return JsonSuccessResult();

                message =
                    "<p><strong>Your password is incorrect.</strong></p><p>Please ensure you've inserted the correct password, or reset your password.</p>";
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }

        return JsonFailResult(modalMessage: message);
    }

    /// <summary>
    /// Checks if a user exists before registering
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostCheckIfUserExists([FromBody] ValidateUserDTO model)
    {
        var message =
            "<p><strong>A user with this Email Address already exists.</strong></p><p>Please choose a different Email Address.</p>";

        try
        {
            var user = await UserManager.FindByEmailAsync(model.Username);

            if (user == null)
                return JsonFailResult(modalMessage: message);

            var passwordOk = true;
            message = "<p><strong>Password validation failed.</strong>";

            foreach (var passwordValidator in UserManager.PasswordValidators)
            {
                if (user != null)
                {
                    var result = await passwordValidator.ValidateAsync(UserManager, user, model.Password);

                    if (result.Succeeded) continue;

                    message = result.Errors.Aggregate(message,
                        (current, error) => current + $"<p>{error.Description}</p>");
                }

                passwordOk = false;
            }

            if (passwordOk)
                return JsonSuccessResult();
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }

        return JsonFailResult(modalMessage: message);
    }

    /// <summary>
    /// Updates a basket in session
    /// </summary>
    /// <param name="basket"></param>
    /// <returns></returns>
    public IActionResult OnPostUpdateBasket([FromBody] BasketModel basket)
    {
        try
        {
            basket.CalculateTotal();
            HttpContext.Session.UpdateObjectInSession("BookingBasket", basket);

            return JsonSuccessResult(new
            {
                Basket = basket
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Creates a Property Booking on success of a Cloudbeds booking
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAddRoomToBasket([FromBody] BasketItemRoomModel request)
    {
        try
        {
            var basket = await BasketService.AddItem(request, PropertyBookingUrl, UserId);

            return JsonSuccessResult(new
            {
                Basket = basket.Item1,
                Item = basket.Item2
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    public async Task<IActionResult> OnPostAddTourDateToBasket([FromBody] BasketItemDateModel request)
    {
        try
        {
            var basket = await BasketService.AddItem(request);

            return JsonSuccessResult(new
            {
                Basket = basket.Item1,
                Item = basket.Item2
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Updates a basket in session
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostRemoveItemFromBasket([FromBody] BasketItemModel request)
    {
        try
        {
            var basket = await BasketService.RemoveItem(request.Id);

            return JsonSuccessResult(new
            {
                Basket = basket
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Add a Guest to a basket item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAddGuestToBasketItem([FromBody] BasketItemGuestModel request)
    {
        try
        {
            var basket = await BasketService.AddGuest(request.ItemId, request);

            return JsonSuccessResult(new
            {
                Basket = basket.Item1,
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AdditionalGuestsPartial.cshtml", basket.Item1)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Returns a fresh modal for adding a new guest.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostGetAddNewGuestModal([FromBody] BasketItemGuestModel request)
    {
        try
        {
            return JsonSuccessResult(new
            {
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AddNewGuestModalPartial.cshtml", new CheckoutGuestComponent())
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Returns a list of guests within the basket.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostGetSelectGuestModal([FromBody] GetBasketItemGuestsRequest request)
    {
        try
        {
            var guests = await BasketService.GetGuests(request);

            return JsonSuccessResult(new
            {
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_SelectGuestsPartial.cshtml", guests)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Removes a Guest from a basket item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostRemoveGuestFromBasketItem([FromBody] BasketItemGuestRequest request)
    {
        try
        {
            var basket = await BasketService.RemoveGuestFromBasketItem(request.ItemId, request.Id);

            return JsonSuccessResult(new
            {
                Basket = basket?.Item1,
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AdditionalGuestsPartial.cshtml", basket.Item1)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Removes a Guest from a basket item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAddExistingGuestToBasketItem([FromBody] AddGuestToBasketItemRequest request)
    {
        try
        {
            var basket = await BasketService.AddExistingGuestToBasketItem(request);

            return JsonSuccessResult(new
            {
                Basket = basket,
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AdditionalGuestsPartial.cshtml", basket)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Retrieves a Guest from a basket item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostEditBasketItemGuest([FromBody] BasketItemGuestRequest request)
    {
        try
        {
            var basket = await BasketService.GetBasket();
            var basketItem = basket.Items.FirstOrDefault(x => x.Id == request.ItemId);

            var guest = basketItem?.Guests!.FirstOrDefault(x => x.Id == request.Id);

            if (guest == null) return JsonFailResult(modalMessage: "No guest found with this Id");

            var model = new CheckoutGuestComponent(
                request.Id,
                request.ItemId,
                guest.FirstName,
                guest.Surname,
                guest.Email,
                guest.DateOfBirth,
                guest.PhoneNumber,
                guest.Nationality,
                guest.Gender
            );

            return JsonSuccessResult(new
            {
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AddNewGuestModalPartial.cshtml", model)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Updates a basket item guest.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostUpdateBasketItemGuest([FromBody] BasketItemGuestModel request)
    {
        try
        {
            var basket = await BasketService.UpdateGuest(request.ItemId, request);

            return JsonSuccessResult(new
            {
                Basket = basket?.Item1,
                Html = await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                    "/Pages/Checkout/_AdditionalGuestsPartial.cshtml", basket.Item1)
            });
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    public async Task<IActionResult> OnPostCreateStripeClientSecret()
    {
        try
        {
            var basket = await BasketService.GetBasket();

            if (!(bool) HttpContextAccessor.HttpContext?.Session.Keys.Contains("GuestId"))
                return new JsonResult(new {clientSecret = ""});

            var guestId = Guid.Parse(HttpContextAccessor.HttpContext?.Session.GetString("GuestId") ?? string.Empty);

            var session = await StripeService.CreateStripeSessionClientSecret(
                new CreateStripeSessionClientSecretRequest()
                {
                    Basket = basket,
                    GuestId = guestId,
                });

            return JsonSuccessResult(new {clientSecret = session.ClientSecret});
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    //
    // /// <summary>
    // /// Retreieves tour dates for a given tour
    // /// </summary>
    // /// <param name="tourId"></param>
    // /// <param name="guestQuantity"></param>
    // /// <returns></returns>
    // public async Task<JsonResult> OnPostGetTourDates([FromBody] GetTourDatesRequest model)
    // {
    //     return new JsonResult(new
    //     {
    //         TourDates = await ApplicationRepository.GetTourDatesAsync(model.TourId, model.GuestQuantity)
    //     });
    // }

    /// <summary>
    /// Creates a Property Booking on success of a Cloudbeds booking
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostCreatePropertyBooking([FromBody] CreateBookingRequest booking)
    {
        try
        {
            await BookingService.CreateAsync(new CreateBookingRequest(
                booking.Description,
                booking.TotalAmount,
                booking.CurrencyCode,
                booking.ItemQuantity,
                booking.IsPaid,
                booking.BookingDate,
                UserId != null ? UserId.Value.ToString() : string.Empty,
                booking.GuestName,
                booking.GuestEmail,
                booking.AdditionalNotes, "Website"));

            return JsonSuccessResult(booking);
        }
        catch (Exception ex)
        {
            Logger.Error("Error: {Message}", ex.Message);
            return StatusCode(500, "There was an error submitting tour request.");
        }
    }

    /// <summary>
    /// Returns a successful json result, with a value and a modalMessage. If modalMessage is supplied, a modal popup will be displayed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="modalMessage"></param>
    /// <returns></returns>
    public IActionResult JsonSuccessResult(object? value = null, string? modalMessage = null)
    {
        return new JsonResult(new JsonResultResponse(true, value, modalMessage));
    }

    /// <summary>
    /// Returns a successful json result, with a modalMessage. A modal popup will be displayed on success.
    /// </summary>
    /// <param name="modalMessage"></param>
    /// <returns></returns>
    public IActionResult JsonSuccessResult(string modalMessage)
    {
        return new JsonResult(new JsonResultResponse(true, null, modalMessage));
    }

    public IActionResult JsonFailResult(object? value = null, string? modalMessage = null)
    {
        return new JsonResult(new JsonResultResponse(false, value, modalMessage));
    }

    #endregion
}