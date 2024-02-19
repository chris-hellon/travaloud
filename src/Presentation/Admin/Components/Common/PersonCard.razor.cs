using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Travaloud.Infrastructure.Identity;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Common;

public partial class PersonCard
{
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;

    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    private string? UserId { get; set; }
    private string? Email { get; set; }
    private string? FullName { get; set; }
    private string? ImageUri { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadUserData();
        }
    }

    private async Task LoadUserData()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var userClaim = authState.User;
        if (userClaim.Identity?.IsAuthenticated == true)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = userClaim.GetUserId();

                if (UserId != null)
                {
                    var user = await UserManager.FindByIdAsync(UserId);

                    FullName = $"{user?.FirstName} {user?.LastName}";
                    Email = userClaim.GetEmail();
                    ImageUri = string.IsNullOrEmpty(userClaim?.GetImageUrl()) ? string.Empty : userClaim?.GetImageUrl();
                    StateHasChanged();
                }
            }
        }
    }
}