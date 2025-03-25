using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Pages.Queries;

namespace FuseHostelsAndTravel.Pages.Contact;

public class IndexModel : ContactPageModel
{
    public IndexModel(IGeneralEnquiriesService generalEnquiriesService) : base(generalEnquiriesService)
    {
        
    }
    
    public override Guid? PageId()
    {
        return new Guid("c919ea71-849c-4cc9-440d-08dc303eaa33");
    }
    
    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords("contact us, get in touch, reach us, email us, phone us");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription("Need to get in touch with Fuse Hostels and Travels? Contact us by email or phone and we'll get back to you as soon as possible.");
    }

    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; } 
    
    public override async Task<IActionResult> OnGetAsync(string? tourName = null, Guid? tourDate = null, int? guestQuantity = null, string? userId = null)
    {
        ViewData["Title"] = "Get In Touch";
        
        await base.OnGetDataAsync();

        ContactComponent = new ContactComponent()
        {
            Tours = Tours,
            Properties = Properties
        };
        OvalContainers = new List<OvalContainerComponent>()
            {
                new("contactPageOvals1", -20, null, -18, null),
                new("contactPageOvals2", null, -20, null, -18)
            };

        return Page();
    }
}
