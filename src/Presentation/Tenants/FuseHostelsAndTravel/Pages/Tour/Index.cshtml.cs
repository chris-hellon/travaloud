using Microsoft.AspNetCore.Mvc;
using Travaloud.Infrastructure.Common.Extensions;

namespace FuseHostelsAndTravel.Pages.Tour;

public class Index : TourPageModel
{
    public Index(ITourEnquiriesService tourEnquiriesService, IToursService toursService) : base(tourEnquiriesService, toursService)
    {
    }

    public override async Task<IActionResult> OnGetAsync(string? tourName = null)
    {
        await OnGetTourAsync(tourName);

        var thumbnailImageSrc = Tour?.ImagePath?.FormatImageUrl(2000, TenantId);

        HeaderBanner = new HeaderBannerComponent(thumbnailImageSrc);

        return Page();
    }
}