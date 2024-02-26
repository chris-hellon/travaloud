using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using Travaloud.Application.Common.Mailing;
using Travaloud.Infrastructure.Identity;

namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class EmailModel : TravaloudBasePageModel
{
    public string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        await base.OnGetDataAsync();

        var email = await UserManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email,
        };

        IsEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await UserManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                protocol: Request.Scheme);

            var mailRequest = new MailRequest(
                to: [Input.NewEmail],
                subject:"Confirm your email",
                body: $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                bcc: [MailSettings.BccAddress.ToString()]);
        
            await MailService.SendAsync(mailRequest);
            
            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return RedirectToPage();
        }

        StatusMessage = "Your email is unchanged.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await UserManager.GetUserIdAsync(user);
        var email = await UserManager.GetEmailAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = userId, code = code },
            protocol: Request.Scheme);

        var mailRequest = new MailRequest(
            to: [Input.NewEmail],
            subject:"Confirm your email",
            body: $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
            bcc: [MailSettings.BccAddress.ToString()]);
        
        await MailService.SendAsync(mailRequest);
        
        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToPage();
    }
}