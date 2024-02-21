using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Account.Pages;

public partial class Login
{
    public string? ErrorMessage;

    [CascadingParameter] private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm] private LoginInputModel Input { get; set; } = new();

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }

    private bool _passwordVisibility;
    
    public InputType PasswordInput = InputType.Password;
    
    public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    public bool BusySubmitting { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        if (TenantInfo?.Id == "root")
        {
            Input.Email = MultitenancyConstants.Root.EmailAddress;
            Input.Password = MultitenancyConstants.DefaultPassword;
        }
    }

    public async Task LoginUser()
    {
        BusySubmitting = true;
        
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        // var result = await SignInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        //
        // if (result.Succeeded)
        // {
        //     Logger.LogInformation("User logged in");
        //
        //     BusySubmitting = false;
        //     StateHasChanged();
        //     
        //     RedirectManager.RedirectTo(ReturnUrl);
        // }
        // else if (result.RequiresTwoFactor)
        // {
        //     BusySubmitting = false;
        //     StateHasChanged();
        //     
        //     RedirectManager.RedirectTo(
        //         "Account/LoginWith2fa",
        //         new Dictionary<string, object?> {["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe});
        // }
        // else if (result.IsLockedOut)
        // {
        //     Logger.LogWarning("User account locked out");
        //     RedirectManager.RedirectTo("Account/Lockout");
        // }
        // else
        // {
        //     ErrorMessage = "Error: Invalid login attempt.";
        // }
        
        var user = await UserManager.FindByEmailAsync(Input.Email);
        
        if (user != null)
        {
            var result = await SignInManager.CheckPasswordSignInAsync(user, Input.Password, false);
        
            if (result.Succeeded)
            {
                if (TenantInfo is {Identifier: not null})
                {
                    await SignInManager.SignInWithClaimsAsync(user, null, new Claim[]
                    {
                        new (TravaloudClaims.Tenant, TenantInfo?.Identifier!)
                    });   
                    
                    Logger.LogInformation("User logged in");
        
                    BusySubmitting = false;
                    StateHasChanged();
            
                    RedirectManager.RedirectTo(ReturnUrl);
                }
                else
                {
                    ErrorMessage = "Error: Invalid login attempt.";
                }
            }
            else if (result.RequiresTwoFactor)
            {
                BusySubmitting = false;
                StateHasChanged();
            
                RedirectManager.RedirectTo(
                    "Account/LoginWith2fa",
                    new Dictionary<string, object?> {["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe});
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User account locked out");
                RedirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                ErrorMessage = "Error: Invalid login attempt.";
            }
        }
        
        BusySubmitting = false;
        StateHasChanged();
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
}