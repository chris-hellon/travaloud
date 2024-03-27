using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.CookiePolicy;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return "cookie policy, website cookies, cookie consent, cookie management";
    }

    public override string MetaDescription()
    {
        return "Learn about Fuse Hostels and Travels' use of cookies on our website and manage your cookie settings with our easy-to-use cookie consent tool.";
    }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["Title"] = "Cookie Policy";
        
        await base.OnGetDataAsync();

        OvalContainers = new List<OvalContainerComponent>()
        {
            new("cookiePolicyPageOvals1", -8, null, -18, null),
            new("cookiePolicyPageOvals2", null, -8, null, -18)
        };

        return Page();
    }
}