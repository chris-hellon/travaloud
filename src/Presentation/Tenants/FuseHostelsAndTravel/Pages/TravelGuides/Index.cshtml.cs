using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Application.Common.Models;

namespace FuseHostelsAndTravel.Pages.TravelGuides;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ITravelGuidesService _travelGuidesService;

    public IndexModel(ITravelGuidesService travelGuidesService)
    {
        _travelGuidesService = travelGuidesService;
    }

    public override Guid? PageId()
    {
        return new Guid("e9274f45-9ebc-477a-165d-08dc304700d3");
    }
    
    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords(  "travel guides, what's on, what's happening, facebook events, fuse events");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription( "What's on at Fuse Hostels and Travel? View our weekly events.");
    }

    [BindProperty] public HeaderBannerComponent? HeaderBanner { get; private set; }
    
    [BindProperty] public PaginationResponse<TravelGuideDto>? TravelGuides { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(int? pageNumber = null)
    {
        ViewData["Title"] = "Travel Guides";

        await base.OnGetDataAsync();
        
        HeaderBanner = new HeaderBannerComponent("TRAVEL GUIDES", "VIETNAM", null, "https://travaloudcdn.azureedge.net/fuse/assets/images/20f8de8d-fea6-403e-8b79-f171b314d7fe.jpg?w=2000", new List<OvalContainerComponent>()
        {
            new("aboutPageHeaderBannerOvals1", 15, null, -30, null)
        });

        TravelGuides = await _travelGuidesService.SearchAsync(new SearchTravelGuidesRequest()
        {
            PageNumber = pageNumber ?? 1,
            PageSize = 7
        });
        
        return Page();
    }
}