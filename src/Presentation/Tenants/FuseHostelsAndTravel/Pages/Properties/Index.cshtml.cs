using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Events.Queries;

namespace FuseHostelsAndTravel.Pages.Properties;

public class IndexModel : TravaloudBasePageModel
{
    private readonly IEventsService _eventsService;

    public IndexModel(IEventsService eventsService) 
    {
        _eventsService = eventsService;
    }

    public override string MetaKeywords()
    {
        return "budget hostels, cheap hostels, backpackers hostels, Vietnam travel";
    }

    public override string MetaDescription()
    {
        return "Our budget hostels in Vietnam offer comfortable and affordable accommodation for backpackers and budget travelers. Book now and start your Vietnam adventure!";
    }

    [BindProperty]
    public HeaderBannerComponent? HeaderBanner { get; private set; }

    [BindProperty]
    public List<ContainerHalfImageTextComponent> HostelsContainers { get; private set; } = [];

    [BindProperty]
    public CarouselCardsComponent? EventsCards { get; private set; }

    [BindProperty]
    public List<NavPill>? NavPills { get; private set; }

    [BindProperty]
    public BookNowComponent? BookNowBanner { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData["Title"] = "Hostels";
        
        await OnGetDataAsync();

        var events = await _eventsService.SearchAsync(new SearchEventsRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });
        
        EventsCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetEventsCarouselCardsAsync(events.Data);

        HeaderBanner = new HeaderBannerComponent("HOSTELS", "VIETNAM", "Looking for good people, great times, and gorgeous scenery? Check out our Vietnam hostels!", "https://images.unsplash.com/photo-1466378284817-a6b7fd50cc68?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=2880&q=80", new List<OvalContainerComponent>()
        {
            new("hostelPageHeaderBannerOvals1", -160, null, -45, null)
        });
        HostelsContainers = await WebComponentsBuilder.FuseHostelsAndTravel.GetHostelsContainersAsync(Properties);
        BookNowBanner = new BookNowComponent(Properties);

        return Page();
    }
}