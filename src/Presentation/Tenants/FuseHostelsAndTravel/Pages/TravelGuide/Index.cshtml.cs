using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Infrastructure.Common.Extensions;

namespace FuseHostelsAndTravel.Pages.TravelGuide;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ITravelGuidesService _travelGuidesService;

    public IndexModel(ITravelGuidesService travelGuidesService)
    {
        _travelGuidesService = travelGuidesService;
    }

    public override string MetaImageUrl()
    {
        return (TravelGuide != null ? TravelGuide.ImagePath : base.MetaImageUrl()) ?? string.Empty;
    }

    public override string MetaKeywords(string? overrideValue = null)
    {
        return (TravelGuide == null ? "budget hostels, cheap hostels, backpackers hostels, Vietnam travel" : TravelGuide.MetaKeywords) ?? string.Empty;
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return (TravelGuide == null ? base.MetaImageUrl() : TravelGuide.MetaDescription) ?? string.Empty;
    }

    [BindProperty] public HeaderBannerComponent? HeaderBanner { get; private set; }

    [BindProperty] public TravelGuideDetailsDto? TravelGuide { get; private set; }
    
    [BindProperty] public TravelGuideDto? RelatedTravelGuide { get; private set; }
    
    [BindProperty] public string? UserName { get; private set; }

    public async Task<IActionResult> OnGetAsync(string title)
    {
        await base.OnGetDataAsync();

        TravelGuide = await _travelGuidesService.GetByFriendlyTitleAsync(title);
        if (TravelGuide == null) return Page();
        
        ViewData["Title"] = TravelGuide.Title;
        
        HeaderBanner = new HeaderBannerComponent(TravelGuide.ImagePath);

        var userTask = UserManager.FindByIdAsync(TravelGuide.CreatedBy);
        var travelGuidesTask = _travelGuidesService.SearchAsync(new SearchTravelGuidesRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });

        await Task.WhenAll(userTask, travelGuidesTask);
        
        if (userTask.Result != null)
            UserName = $"{userTask.Result.FirstName} {userTask.Result.LastName}";

        var travelGuides = travelGuidesTask.Result.Data;

        if (travelGuides.Count == 0 || travelGuides.Count == 1) return Page();
        
        travelGuides = travelGuides.Where(x => x.Id != TravelGuide.Id).ToList();
        travelGuides.Shuffle();
        
        RelatedTravelGuide = travelGuides.Take(1).First();

        return Page();
    }

}