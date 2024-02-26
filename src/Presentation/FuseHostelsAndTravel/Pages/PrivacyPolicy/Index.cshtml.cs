using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.PrivacyPolicy;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return "privacy policy, data protection, personal information, travel privacy";
    }

    public override string MetaDescription()
    {
        return "At Fuse Hostels and Travels, we take your privacy seriously. Our privacy policy outlines how we protect and handle your personal information when you book with us.";
    }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["Title"] = "Privacy Policy";
        
        await base.OnGetDataAsync();

        OvalContainers = new List<OvalContainerComponent>()
        {
            new("privacyPolicyPageOvals1", -8, null, -18, null),
            new("privacyPolicyPageOvals2", null, -8, null, -18)
        };

        return Page();
    }
}