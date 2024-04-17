using Microsoft.Extensions.Options;
using Travaloud.Application.PaymentProcessing;

namespace Travaloud.Tenants.SharedRCL.Pages.Payment;

public class IndexModel : TravaloudBasePageModel
{
    private readonly StripeSettings _stripeSettings;
    public string StripePublicKey { get; set; }

    public IndexModel(IOptions<StripeSettings> stripeOptions, IStripeService stripeService)
    {
        _stripeSettings = stripeOptions.Value;
        StripePublicKey = _stripeSettings.ApiPublishKey;
    }
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        return Page();
    }
}