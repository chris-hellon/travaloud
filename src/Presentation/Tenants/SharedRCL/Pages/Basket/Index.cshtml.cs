using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Dto;

namespace Travaloud.Tenants.SharedRCL.Pages.Basket;

public class IndexModel : TravaloudBasePageModel
{
    public BasketModel? Basket { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        Basket = await BasketService.GetBasket();

        return Page();
    }
}