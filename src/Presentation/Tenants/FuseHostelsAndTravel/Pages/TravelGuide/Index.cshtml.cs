using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Images.Dto;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Application.Common.Extensions;
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
    
    [BindProperty] public List<TravelGuideDto> RelatedTravelGuides { get; private set; }
    
    [BindProperty] public CarouselCardsComponent? RelatedTravelGuidesCards { get; private set; }
    
    [BindProperty] public string? UserName { get; private set; }

    [BindProperty] public List<NavPill>? NavPills { get; private set; }
    
    [BindProperty] public FullImageCarouselComponent? CarouselComponent { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(string title)
    {
        await base.OnGetDataAsync();

        TravelGuide = await _travelGuidesService.GetByFriendlyTitleAsync(title);
        if (TravelGuide == null) return Page();
        
        ViewData["Title"] = TravelGuide.Title;
        //
        // HeaderBanner = new HeaderBannerComponent(TravelGuide.ImagePath)
        // {
        //     SlideshowImages = [],
        //     FullHeight = true
        // };
        //
        // if (!string.IsNullOrEmpty(TravelGuide.ImagePath))
        //     HeaderBanner.SlideshowImages.Add(TravelGuide.ImagePath);
        //
        // HeaderBanner.SlideshowImages.AddRange(TravelGuide.TravelGuideGalleryImages.Select(x => $"{x.ImagePath}?w=2000").ToList());
        //

        //HeaderBanner = new HeaderBannerComponent(TravelGuide.Title, null, null, TravelGuide.ImagePath);

        var htmlContent = TravelGuide.Description;
        NavPills = ExtractNavPills(ref htmlContent);
        NavPills.Insert(0, new NavPill(TravelGuide.SubTitle, 1400));
        TravelGuide.Description = htmlContent;
        
        var userTask = UserManager.FindByIdAsync(TravelGuide.CreatedBy);
        var travelGuidesTask = _travelGuidesService.SearchAsync(new SearchTravelGuidesRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });
        

        await Task.WhenAll(userTask, travelGuidesTask);
        
        if (userTask.Result != null)
            UserName = $"{userTask.Result.FirstName} {userTask.Result.LastName}";

        var carouselImages = await GetHomePageCarouselImages();
        carouselImages.Insert(0, new ImageDto()
        {
            ImagePath =
                $"{TravelGuide.ImagePath}?w=2000",
            ThumbnailImagePath =
                $"{TravelGuide.ImagePath}?w=2000",
            AltText = $"{TravelGuide.Title} image gallery 1"
        });
        CarouselComponent = new FullImageCarouselComponent(carouselImages)
        {
            FullCoverOneTitle = true,
            Title = TravelGuide.Title,
            SubTitle = $"Published <u>{TravelGuide?.CreatedOn.ToShortDateString()}</u><br />by {UserName}"
        };
        
        var travelGuides = travelGuidesTask.Result.Data;

        if (travelGuides.Count == 0 || travelGuides.Count == 1) return Page();
        
        travelGuides = travelGuides.Where(x => x.Id != TravelGuide.Id).ToList();
        travelGuides.Shuffle();
        
        RelatedTravelGuides = travelGuides.Take(3).ToList();
        RelatedTravelGuidesCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetTravelGuidesCarouselCardsAsync(RelatedTravelGuides);
        
        return Page();
    }

    public Task<List<ImageDto>> GetHomePageCarouselImages()
    {
        var (title, subTitle, subSubTitle) = PageDetails.GetTitleSubTitleAndSubSubTitle(
            TravelGuide.Title,
            TravelGuide.CreatedOn.ToShortDateString(), 
            $"by {TravelGuide.CreatedBy}");

        var index = 1;
        
        return Task.FromResult(TravelGuide.TravelGuideGalleryImages.Select(x => 
            new ImageDto() {
                //ImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                //ThumbnailImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                ImagePath =
                    $"{x.ImagePath}?w=2000",
                ThumbnailImagePath =
                    $"{x.ImagePath}?w=2000",
                AltText = $"{TravelGuide.Title} image gallery {index++}"
        }).ToList()) ;
    }
    
    private static List<NavPill> ExtractNavPills(ref string htmlContent)
    {
        var navPills = new List<NavPill>();
        
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var headers = doc.DocumentNode.SelectNodes("//h2 | //h3 | //h4 | //h5");

        if (headers != null)
        {
            int position = 1600;

            foreach (var header in headers)
            {
                var title = header.InnerText.Trim();
                var navPill = new NavPill(title, position);
                
                // Set the Id attribute of the HTML element
                header.SetAttributeValue("id", navPill.IdTitle);
                
                navPills.Add(navPill);
                position += 200;
            }

            // Save the modified HTML back to the input string
            htmlContent = doc.DocumentNode.OuterHtml;
        }

        return navPills;
    }
}