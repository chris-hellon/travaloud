using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Events.Queries;


namespace FuseHostelsAndTravel.Pages.Events;

public class IndexModel  : TravaloudBasePageModel
{
    private readonly IEventsService _eventsService;

    public IndexModel(IEventsService eventsService) 
    {
        _eventsService = eventsService;
    }

    public override Guid? PageId()
    {
        return new Guid("ce9f04e3-abff-4963-165e-08dc304700d3");
    }

    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords( "events, what's on, what's happening, facebook events, fuse events");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription("Find and join events with FUSE Hostels & Travel. Discover unforgettable experiences. Click here to explore and book today!");
    }

    [BindProperty] public HeaderBannerComponent? HeaderBanner { get; private set; }
    
    [BindProperty]
    public CarouselCardsComponent? EventsCards { get; private set; }
    
    [BindProperty]
    public List<NavPill>? NavPills { get; private set; }
    
    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["Title"] = "Events";
        
        await base.OnGetDataAsync();

        NavPills = new List<NavPill>()
        {
            new("WHAT'S ON", 1400),
            new("STAY UP TO DATE", 1600),
            new("HOSTELS", 1800)
        };
        
        HeaderBanner = new HeaderBannerComponent("AT FUSE HOSTELS & TRAVEL?", "WHAT'S ON", null, "https://travaloudcdn.azureedge.net/fuse/assets/images/26885dcf-2138-4b69-8540-5203a35e65dd.jpg", new List<OvalContainerComponent>()
        {
            new("aboutPageHeaderBannerOvals1", 15, null, -30, null)
        });
        
        OvalContainers = new List<OvalContainerComponent>()
        {
            new("contactPageOvals1", -20, null, -18, null),
            new("contactPageOvals2", null, -20, null, -18)
        };

        var events = await _eventsService.SearchAsync(new SearchEventsRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });
        
        var eventsCards = WebComponentsBuilder.FuseHostelsAndTravel.GetEventsCarouselCardsAsync(events.Data);

        await eventsCards;

        EventsCards = eventsCards.Result;
        
        return Page();
    }
}