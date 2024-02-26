using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.TermsAndConditions;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return "terms and conditions, hostel booking, travel terms and conditions, hostel policies";
    }

    public override string MetaDescription()
    {
        return "Check out our terms and conditions before booking your stay at Fuse Hostels and Travels. We've outlined our policies to ensure a smooth and enjoyable experience during your travels.";
    }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; } 

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["Title"] = "Terms & Conditions";
        
        await base.OnGetDataAsync();

        OvalContainers = new List<OvalContainerComponent>()
        {
            new("termsAndConditionsPageOvals1", -8, null, -18, null),
            new("termsAndConditionsPageOvals2", null, -8, null, -18)
        };

        return Page();
    }
}