using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Identity.Users.Password;
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
    [Parameter] public ChangePasswordRequest ChangePasswordRequest { get; set; } = new();

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

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    
    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequest {ActivateUser = _active, UserId = Id};
        await ServiceHelper.ExecuteCallGuardedAsync(() => UsersService.ToggleStatusAsync(request), Snackbar, Logger);

        if (ChangePasswordRequest is {NewPassword: not null, ConfirmNewPassword: not null})
        {
            await ServiceHelper.ExecuteCallGuardedAsync(() => UsersService.ChangePasswordAsync(ChangePasswordRequest, Id), Snackbar, Logger, "Password Updated Successfully.");
        }
        
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
    
    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }

        StateHasChanged();
    }
}