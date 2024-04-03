using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Dto;

namespace Travaloud.Tenants.SharedRCL.Pages.TourBooking;

public class IndexModel : TravaloudBasePageModel
{
    public BasketModel? Basket { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        Basket = HttpContext.Session.GetOrCreateObjectFromSession<BasketModel>("BookingBasket");
        // Basket.Items = Basket.Items.Select(x =>
        // {
        //     x.TourDates = x.TourDates.Select(td =>
        //     {
        //         td.IsConfirmationPage = true;
        //         return td;
        //     }).ToList();
        //     return x;
        // }).ToList();

        return Page();
    }
}