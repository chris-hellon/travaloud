using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.TermsAndConditions;

public class IndexModel : TravaloudBasePageModel
{
    public override Guid? PageId()
    {
        return new Guid("82b60091-9fba-42f0-440e-08dc303eaa33");
    }
    
    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords(  "terms and conditions, hostel booking, travel terms and conditions, hostel policies");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription( "Check out our terms and conditions before booking your stay at Fuse Hostels and Travels. We've outlined our policies to ensure a smooth and enjoyable experience during your travels.");
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