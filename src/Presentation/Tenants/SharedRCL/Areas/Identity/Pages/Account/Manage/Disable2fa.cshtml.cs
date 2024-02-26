namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class Disable2faModel : TravaloudBasePageModel
{
    public async Task<IActionResult> OnGet()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        if (!await UserManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException($"Cannot disable 2FA for user as it's not currently enabled.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        var disable2faResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred disabling 2FA.");
        }

        StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
        return RedirectToPage("./TwoFactorAuthentication");
    }
}