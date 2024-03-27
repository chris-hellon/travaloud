using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Application.Catalog.Properties.Dto;

namespace FuseHostelsAndTravel.Pages.Property;

public class IndexModel : TravaloudBasePageModel
{
    private readonly IEventsService _eventsService;
    private readonly IPropertiesService _propertiesService;

    public IndexModel(IEventsService eventsService, IPropertiesService propertiesService) 
    {
        _eventsService = eventsService;
        _propertiesService = propertiesService;
    }

    public override string MetaKeywords()
    {
        return (Property == null ? "budget hostels, cheap hostels, backpackers hostels, Vietnam travel" : Property.MetaKeywords) ?? string.Empty;
    }

    public override string MetaDescription()
    {
        return (Property == null ? "Our budget hostels in Vietnam offer comfortable and affordable accommodation for backpackers and budget travelers. Book now and start your Vietnam adventure!" : Property.MetaDescription) ?? string.Empty;
    }

    public override string MetaImageUrl()
    {
        return (Property == null ? base.MetaImageUrl() : Property.ImagePath) ?? string.Empty;
    }

    [BindProperty]
    public PropertyDetailsDto? Property { get; private set; }

    [BindProperty]
    public ContainerHalfImageRoundedTextComponent? IntroductionBanner { get; private set; }

    [BindProperty]
    public HeaderBannerComponent? HeaderBanner { get; private set; }

    [BindProperty]
    public List<ContainerHalfImageTextComponent> HostelsContainers { get; private set; } = new();

    [BindProperty]
    public CarouselCardsComponent? ToursCards { get; private set; }

    [BindProperty]
    public FullImageCardComponent? AccommodationCards { get; private set; }

    [BindProperty]
    public CarouselCardsComponent? EventsCards { get; private set; }

    [BindProperty]
    public List<NavPill>? NavPills { get; private set; }

    [BindProperty]
    public NavPillsComponent? DirectionsNavPills { get; private set; }

    [BindProperty]
    public BookNowComponent? BookNowBanner { get; private set; }

    [BindProperty]
    public FeaturesTableComponent? FacilitiesTable { get; private set; }

    public async Task<IActionResult> OnGet(string? propertyName = null)
    {
        await OnGetDataAsync();

        var events = await _eventsService.SearchAsync(new SearchEventsRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });
        
        EventsCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetEventsCarouselCardsAsync(events.Data);

        if (propertyName == null)
        {
            HeaderBanner = new HeaderBannerComponent("HOSTELS", "VIETNAM", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.", "https://images.unsplash.com/photo-1466378284817-a6b7fd50cc68?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2880&q=80", new List<OvalContainerComponent>()
            {
                new("hostelPageHeaderBannerOvals1", -160, null, -45, null)
            });
            
            if (Properties == null) return Page();
            
            HostelsContainers = await WebComponentsBuilder.FuseHostelsAndTravel.GetHostelsContainersAsync(Properties);
            BookNowBanner = new BookNowComponent(Properties);
        }
        else
        {
            var property = Properties?.FirstOrDefault(x => x.FriendlyUrl == propertyName);

            if (property != null)
            {
                Property = await _propertiesService.GetAsync(property.Id);

                if (Property != null)
                {
                    var pageTitle = Property.PageTitle ?? Property.Name;
                    var pageSubTitle = Property.PageSubTitle ?? "";

                    var thumbnailImageSrc = Property.ImagePath?.Replace("tr=w-2000", $"w=2000").Replace("https://ik.imagekit.io/rqlzhe7ko/", $"https://travaloudcdn.azureedge.net/fuse/assets/images/");

                    if (Property.Images != null && Property.Images.Any())
                    {
                        HeaderBanner = new HeaderBannerComponent(pageTitle, pageSubTitle, null, Property.Images.Select(x => x.ImagePath).ToList(), new List<OvalContainerComponent>()
                        {
                            new("hostelPageHeaderBannerOvals1", 15, null, -30, null)
                        });
                    }
                    else
                    {
                        HeaderBanner = new HeaderBannerComponent(pageTitle, pageSubTitle, null, thumbnailImageSrc, new List<OvalContainerComponent>()
                        {
                            new("hostelPageHeaderBannerOvals1", 15, null, -30, null)
                        });
                    }

                    if (property.Id == new Guid("8E1F2B64-6EF8-4321-B6AB-B9B578E0E6CB"))
                    {
                        HeaderBanner.BackgroundTop = 50;
                    }

                    var directionsNavPills = Task.Run(() => WebComponentsBuilder.FuseHostelsAndTravel.GetHostelDirectionsNavPillsAsync(Property)) ;
                    var toursCards = Task.Run(() => WebComponentsBuilder.FuseHostelsAndTravel.GetToursCarouselCardsAsync(Tours, "onScroll", (property.Id == new Guid("8E1F2B64-6EF8-4321-B6AB-B9B578E0E6CB") ? "NHA TRANG" : "HOI AN") + " EXPERIENCES", null));
                    var accommodationCards = Task.Run(() => WebComponentsBuilder.FuseHostelsAndTravel.GetHostelAccommodationCardsAsync(
                        Property.Rooms!, "onScroll", "ACCOMMODATION",
                        Property.Id == new Guid("7335037B-853F-4E66-B61B-8E02BDCA9251") ? $"All private & shared rooms at FUSE {property.Name} Hostel come with pool or ocean views as standard so there is no better place to stay if beach vibes are your thing." :
                        property.Id == new Guid("8E1F2B64-6EF8-4321-B6AB-B9B578E0E6CB") ? $"FUSE {property.Name} Hostel features an outdoor swimming pool, shared lounge, a terrace and restaurant in Nha Trang. Featuring a bar, the hostel is close to several noted attractions." :
                        "The ultimate cure for your backpacker's fatigue, conveniently located at the edge of Hoi An's Old Town and equipped with all the amenities you could possibly need!"));

                    var facilitiesTable = Task.Run(() => WebComponentsBuilder.FuseHostelsAndTravel.GetHostelFacilitiesAsync(Property));

                    await Task.WhenAll(directionsNavPills, toursCards, accommodationCards, facilitiesTable);

                    DirectionsNavPills = directionsNavPills.Result;
                    ToursCards = toursCards.Result;
                    AccommodationCards = accommodationCards.Result;
                    FacilitiesTable = facilitiesTable.Result;

                    var introductionBannerImage = Property.Id == new Guid("8e1f2b64-6ef8-4321-b6ab-b9b578e0e6cb") ? "https://travaloudcdn.azureedge.net/fuse/assets/images/99c08e57-cd8a-4fd0-aaad-ffb87a6c581d.jpg?w=1600" : "https://travaloudcdn.azureedge.net/fuse/assets/images/OT soft opening _ more (40 of 154).jpg?w=600";
                    
                    IntroductionBanner = new ContainerHalfImageRoundedTextComponent(new List<string>() { "ABOUT" }, null, Property.Description,
                            introductionBannerImage, null, new List<OvalContainerComponent>()
                            {
                                new("hostelPageIntroductionOvals1", 15, null, null, -28),
                                new("hostelPageIntroductionOvals2", null, 15, null, 18)
                            }, videoSrc: Property.VideoPath)
                        { AnimationStart = "onLoad" };
                    NavPills = new List<NavPill>()
                    {
                        new("ABOUT", 1400),
                        new("BOOK NOW", 1600),
                        new("ACCOMMODATION", 1800),
                        new($"{Property.PageTitle?.ToUpper().Replace("HOSTEL", "")} EXPERIENCES", 1800),
                        new($"GETTING TO {pageTitle.ToUpper()}", 2000),
                    };
                }
            }

            if (Properties != null && Property != null)
            {
                BookNowBanner = new BookNowComponent(Properties, Property.Id);
            }   
        }

        return Page();
    }


    //public async Task GetPropertyInformation(Property property)
    //{
    //    var propertyId = property.Id;

    //    property.Rooms = await DapperConnection.ExecuteGetStoredProcedureList<PropertyRoom>("GetPropertyRooms", new { PropertyId = propertyId });
    //    property.Facilities = await DapperConnection.ExecuteGetStoredProcedureList<PropertyFacility>("GetPropertyFacilities", new { PropertyId = propertyId });
    //    property.Directions = await GetPropertyDirections(propertyId);
    //    property.Tours = await GetPropertyTours(propertyId);
    //}
}