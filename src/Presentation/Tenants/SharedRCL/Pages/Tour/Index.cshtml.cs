using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Pages.Tour;

public class IndexModel : TourPageModel
{
    private readonly IToursService _toursService;

    public IndexModel(IToursService toursService)
    {
        _toursService = toursService;
    }

    public override string Subject()
    {
        return "Tour Booking Enquiry";
    }

    public override string TemplateName()
    {
        return "TourEnquiryTemplate";
    }

    public override string MetaKeywords()
    {
        return Tour?.MetaKeywords ?? base.MetaKeywords();
    }

    public override string MetaDescription()
    {
        return Tour?.MetaDescription ?? base.MetaDescription();
    }

    public override string MetaImageUrl()
    {
        return Tour?.ImagePath ?? base.MetaImageUrl();
    }

    [BindProperty]
    public TourDetailsDto? Tour { get; set; }

    [BindProperty]
    public GenericCardsComponent? RelatedToursCards { get; private set; }

    [BindProperty]
    public string PageTitle { get; private set; }

    [BindProperty]
    public HeaderBannerComponent HeaderBanner { get; private set; }

    [BindProperty]
    public BookNowComponent? BookNowComponent { get; private set; }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null)
    {
        await OnGetDataAsync();

        var tour = Tours?.FirstOrDefault(x => x.FriendlyUrl == tourName);

        if (tour == null) return Page();
        {
            Tour = await _toursService.GetAsync(tour.Id);

            if (Tour == null) return Page();
            
            ViewData["Title"] = Tour?.Name;

            EnquireNowComponent = new EnquireNowComponent()
            {
                TourName = Tour.Name,
                TourId = Tour.Id
            };

            BookNowComponent = new BookNowComponent(Tours, Tour.Id);

            if (TenantId == "fuse")
            {
                var thumbnailImageSrc = Tour.ImagePath?.Replace("tr=w-2000", $"w=2000").Replace("https://ik.imagekit.io/rqlzhe7ko/fuse/", $"https://travaloudcdn.azureedge.net/fuse/assets/images/").Replace("https://travaloud.azureedge.net", "https://travaloudcdn.azureedge.net");

                HeaderBanner = new HeaderBannerComponent(thumbnailImageSrc);
            }
            else
            {
                //TODO: Implement for VBH and Uncut
                // if (ToursWithCategories != null && ToursWithCategories.Any())
                // {
                //     var relatedTours = ToursWithCategories.Where(t => t.Id != Tour.Id).ToList();
                //
                //     if (relatedTours != null)
                //     {
                //         relatedTours.Shuffle();
                //         relatedTours = relatedTours.Take(3).ToList();
                //
                //         RelatedToursCards = WebComponentsBuilder.GetToursWithCategoriesGenericCards(TenantId, relatedTours, null, true, "More Like This", true);
                //     }
                // }
                //
                // Tour.TourItineraries = await ApplicationRepository.GetTourItinerariesAsync(Tour.Id);
                    
                var tourCategoryNavLink = NavigationSettings?.NavigationLinks.FirstOrDefault(x => x.ChildrenEntity is "Tours" or "ToursWithCategories");
                if (tourCategoryNavLink != null)
                    PageTitle = tourCategoryNavLink.Title;
            }
        }

        return Page();
    }
}