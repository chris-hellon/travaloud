using Microsoft.AspNetCore.Http.Extensions;
using Travaloud.Infrastructure.Identity;

namespace Travaloud.Tenants.SharedRCL.Pages.PropertyBooking;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return $"{PropertyName}, hostel booking, budget travel, backpacker accommodation";
    }

    public override string MetaDescription()
    {
        return $"Looking for budget-friendly backpacker accommodation in Vietnam? Look no further than {TenantName} - {PropertyName}. Book your stay now for an unforgettable experience.";
    }

    private ApplicationUser? ApplicationUser { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public Guid PropertyId { get; set; }
    public string IframeUrl { get; set; }

    [BindProperty]
    public HeaderBannerComponent HeaderBanner { get; private set; }

    public async Task<IActionResult> OnGetAsync(string propertyName, string? checkInDate = null, string? checkOutDate = null, string? userId = null)
    {
        await OnGetDataAsync();

        var url = Request.GetEncodedUrl();

        LoginModal.ReturnUrl = url;
        RegisterModal.ReturnUrl = url;

        if (userId != null)
            ApplicationUser = await UserManager.FindByIdAsync(userId);

        if (Properties == null) return LocalRedirect("/error");
        
        var property = Properties.FirstOrDefault(h => h.Name.UrlFriendly() == propertyName.UrlFriendly());

        if (property == null) return LocalRedirect("/error");
        
        var pageTitle = property.PageTitle ?? property.Name;
        var pageSubTitle = property.PageSubTitle ?? "";

        HeaderBanner = new HeaderBannerComponent(pageTitle, pageSubTitle, null, property.ImagePath, new List<OvalContainerComponent>() { new("hostelPageHeaderBannerOvals1", 15, null, -30, null) });
        PropertyName = property.Name;
        PropertyId = property.Id;
        IframeUrl = $"https://hotels.cloudbeds.com/reservation/{property.CloudbedsKey}";

        if (ApplicationUser != null)
        {
            IframeUrl += "?";

            if (!string.IsNullOrEmpty(ApplicationUser.FirstName))
                IframeUrl += $"firstName={ApplicationUser.FirstName}";

            if (!string.IsNullOrEmpty(ApplicationUser.LastName))
                IframeUrl += $"&lastName={ApplicationUser.LastName}";

            if (!string.IsNullOrEmpty(ApplicationUser.Email))
                IframeUrl += $"&email={ApplicationUser.Email}";

            if (!string.IsNullOrEmpty(ApplicationUser.Nationality))
                IframeUrl += $"&country={ApplicationUser.Nationality}";

            if (!string.IsNullOrEmpty(ApplicationUser.PhoneNumber))
                IframeUrl += $"&phone={ApplicationUser.PhoneNumber}";
        }

        if (string.IsNullOrEmpty(checkInDate) && string.IsNullOrEmpty(checkOutDate)) return Page();
        
        IframeUrl += "#";

        if (!string.IsNullOrEmpty(checkInDate))
            IframeUrl += $"&checkin={checkInDate}";

        if (!string.IsNullOrEmpty(checkOutDate))
            IframeUrl += $"&checkout={checkOutDate}";

        return Page();

    }
}