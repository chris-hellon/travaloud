using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.Contact;

public class IndexModel : ContactPageModel
{
    public IndexModel(IGeneralEnquiriesService generalEnquiriesService) : base(generalEnquiriesService)
    {
        
    }
    
    public override string MetaKeywords()
    {
        return "contact us, get in touch, reach us, email us, phone us";
    }

    public override string MetaDescription()
    {
        return "Need to get in touch with Fuse Hostels and Travels? Contact us by email or phone and we'll get back to you as soon as possible.";
    }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; } 
    
    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null)
    {
        ViewData["Title"] = "Get In Touch";
        
        await base.OnGetDataAsync();

        OvalContainers = new List<OvalContainerComponent>()
            {
                new("contactPageOvals1", -20, null, -18, null),
                new("contactPageOvals2", null, -20, null, -18)
            };

        return Page();
    }
}
