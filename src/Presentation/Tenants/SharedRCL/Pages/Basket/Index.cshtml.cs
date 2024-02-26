namespace Travaloud.Tenants.SharedRCL.Pages.Basket;

public class IndexModel : TravaloudBasePageModel
{
    public BasketModel? Basket { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        Basket = HttpContext.Session.GetOrCreateObjectFromSession<BasketModel>("tourBookingBasket");

        return Page();
    }
}