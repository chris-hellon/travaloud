using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel : TravaloudBasePageModel
{
    public async Task<IActionResult> OnGet()
    {
        await base.OnGetDataAsync();

        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        // Only include personal data for download
        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(IdentityUser).GetProperties().Where(
            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (var p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        var logins = await UserManager.GetLoginsAsync(user);
        foreach (var l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add($"Authenticator Key", await UserManager.GetAuthenticatorKeyAsync(user));

        Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}