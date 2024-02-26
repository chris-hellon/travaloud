using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.Tours;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return "Vietnam tours, budget tours, adventure tours, group tours, Vietnam travel";
    }

    public override string MetaDescription()
    {
        return "Explore Vietnam with our affordable and exciting budget tours. Join a group tour or customize your own adventure and discover all that Vietnam has to offer.";
    }

    [BindProperty]
    public AlternatingCardHeightComponent? ToursCards { get; private set; }

    [BindProperty]
    public HeaderBannerComponent? HeaderBanner { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData["Title"] = "Tours";
        
        await base.OnGetDataAsync();

        HeaderBanner = new HeaderBannerComponent("HOI AN & NHA TRANG", "HOSTEL TOURS", "", "https://travaloudcdn.azureedge.net/fuse/assets/images/146f0d66-95b1-42fb-9902-d5466890e60d.jpg?w=2000", new List<OvalContainerComponent>()
        {
            new("hostelPageHeaderBannerOvals1", -160, null, -45, null)
        });
        ToursCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetToursCardsAsync(Tours, "onLoad");

        return Page();
    }
}