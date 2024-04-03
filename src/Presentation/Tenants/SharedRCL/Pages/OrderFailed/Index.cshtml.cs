namespace Travaloud.Tenants.SharedRCL.Pages.OrderFailed;

public class IndexModel  : TravaloudBasePageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        return Page();
    }
}