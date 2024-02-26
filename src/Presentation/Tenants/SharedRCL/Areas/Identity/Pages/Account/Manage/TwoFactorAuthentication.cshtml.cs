namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class TwoFactorAuthenticationModel : TravaloudBasePageModel
{
    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        HasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(user) != null;
        Is2faEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
        IsMachineRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user);
        RecoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        await SignInManager.ForgetTwoFactorClientAsync();
        StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return RedirectToPage();
    }
}