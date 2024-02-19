using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Staff;

public partial class StaffRoles
{
    [Parameter] public string? Id { get; set; }
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject] protected IUserService UsersService { get; set; } = default!;

    private List<UserRoleDto> _userRolesList = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditUsers;
    private bool _canSearchRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        _canEditUsers = await AuthService.HasPermissionAsync(state.User, TravaloudAction.Update, TravaloudResource.Users);
        _canSearchRoles = await AuthService.HasPermissionAsync(state.User, TravaloudAction.View, TravaloudResource.UserRoles);

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => UsersService.GetAsync(Id!),Snackbar, Logger)
            is { } user)
        {
            _title = $"{user.FirstName} {user.LastName}";
            _description = string.Format(L["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);

            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () => UsersService.GetRolesAsync(user.Id.ToString()),Snackbar,Logger)
                is ICollection<UserRoleDto> response)
            {
                _userRolesList = response.ToList();
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        var request = new UserRolesRequest()
        {
            UserRoles = _userRolesList
        };

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => UsersService.AssignRolesAsync(Id!, request),
                Snackbar,
                Logger,
                successMessage: L["Updated Staff Roles."])
            is not null)
        {
            await LoadingService.ToggleLoaderVisibility(false);
            NavigationManager.NavigateTo("/staff");
        }
    }

    private bool Search(UserRoleDto userRole) =>
        string.IsNullOrWhiteSpace(_searchString)
        || userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}