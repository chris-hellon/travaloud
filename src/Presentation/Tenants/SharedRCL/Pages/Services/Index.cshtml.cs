namespace Travaloud.Tenants.SharedRCL.Pages.Services;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaDescription()
    {
        return $"At {TenantName}, we offer a range of services to make your stay as comfortable and convenient as possible. From airport transfers, to visa letters, we've got you covered.";
    }

    public override string MetaKeywords()
    {
        return $"{TenantName}, hostel services, laundry services, airport transfers, travel services, backpacker-friendly";
    }

    [BindProperty]
    public GenericCardsComponent ServicesCards { get; set; }

    [BindProperty]
    public CarouselCardsComponent ServicesCarousel { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        await base.OnGetDataAsync();

        //ServicesCards = WebComponentsBuilder.UncutTravel.GetServicesGenericCards(Services, null);
        ServicesCarousel = WebComponentsBuilder.VietnamBackpackerHostels.GetServicesCarouselCards(Services, "", null);
        return Page();
    }
}