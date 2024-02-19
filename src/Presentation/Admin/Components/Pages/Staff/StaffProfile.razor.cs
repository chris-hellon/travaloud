using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Staff;

public partial class StaffProfile
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject] protected IUserService UsersService { get; set; } = default!;

    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }

    private bool _active;
    private bool _emailConfirmed;
    private char _firstLetterOfName;
    private string? _firstName;
    private string? _lastName;
    private string? _phoneNumber;
    private string? _email;
    private string? _imageUrl;
    private bool _loaded;
    private bool _canToggleUserStatus;

    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequest {ActivateUser = _active, UserId = Id};
        await ServiceHelper.ExecuteCallGuardedAsync(() => UsersService.ToggleStatusAsync(request), Snackbar, Logger);
        NavigationManager.NavigateTo("/staff");
    }

    [Parameter] public string? ImageUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => UsersService.GetAsync(Id!),Snackbar, Logger)
            is { } user)
        {
            _firstName = user.FirstName;
            _lastName = user.LastName;
            _email = user.Email;
            _phoneNumber = user.PhoneNumber;
            _active = user.IsActive;
            _emailConfirmed = user.EmailConfirmed;
            _imageUrl = string.IsNullOrEmpty(user.ImageUrl) ? string.Empty : user.ImageUrl;
            Title = $"{_firstName} {_lastName}'s {_localizer["Profile"]}";
            Description = _email;
            if (_firstName?.Length > 0)
            {
                _firstLetterOfName = _firstName.ToUpper().FirstOrDefault();
            }
        }

        var state = await AuthState.GetAuthenticationStateAsync();
        _canToggleUserStatus = await AuthService.HasPermissionAsync(state.User, TravaloudAction.Update, TravaloudResource.Users);
        _loaded = true;
    }
}