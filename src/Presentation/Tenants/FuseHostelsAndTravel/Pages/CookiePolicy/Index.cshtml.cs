using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.CookiePolicy;

public class IndexModel : TravaloudBasePageModel
{
    public override Guid? PageId()
    {
        return new Guid("393dbb36-9e5a-4d06-4410-08dc303eaa33");
    }

    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords("contact us, get in touch, reach us, email us, phone us");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription("Learn about Fuse Hostels and Travels' use of cookies on our website and manage your cookie settings with our easy-to-use cookie consent tool.");
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