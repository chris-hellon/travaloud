using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using BlazorTemplater;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Serilog.Core;
using Travaloud.Application.Common.Mailing;
using Travaloud.Infrastructure.Identity;
using Travaloud.Infrastructure.Multitenancy;
using ILogger = Serilog.ILogger;

namespace Travaloud.Admin.Components.Account.Pages;

public partial class ForgotPassword
{
    [SupplyParameterFromForm] private InputModel Input { get; set; } = new();

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    private bool BusySubmitting { get; set; }
    
    private async Task OnValidSubmitAsync()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
        {
            RedirectManager.RedirectTo("account/forgot-password-confirmation");
        }

        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("account/reset-password").AbsoluteUri,
            new Dictionary<string, object?> {["code"] = code});

        if (user.Email != null)
        {
            var mailHtml = new ComponentRenderer<EmailTemplates.ForgotPasswordConfirmation>()
                .Set(c => c.Model, new PasswordResetModel(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl), TenantInfo!))
                .Render();
            
            var mailRequest = new MailRequest(
                [user.Email],
                L["Reset Password"],
                mailHtml);

            await MailService.SendAsync(mailRequest);
        }

        RedirectManager.RedirectTo("account/forgot-password-confirmation");
    }
    
    private sealed class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; } = "";
    }

    public class PasswordResetModel
    {
        public ApplicationUser User { get; set; }
        public TravaloudTenantInfo TenantInfo { get; set; }
        public string Email { get; set; }
        public string ResetLink { get; set; }
        
        public PasswordResetModel(ApplicationUser user, string email, string resetLink, TravaloudTenantInfo tenantInfo)
        {
            User = user;
            Email = email;
            ResetLink = resetLink;
            TenantInfo = tenantInfo;
        }

    }
}