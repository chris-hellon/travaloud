using Mapster;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Application.Identity.Roles;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.Permissions;

public partial class Permission
{
    [Parameter]
    public string Id { get; set; } = default!; 
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IRoleService RoleService { get; set; } = default!;

    private Dictionary<string, List<PermissionViewModel>> _groupedRoleClaims = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    static Permission() => TypeAdapterConfig<TravaloudPermission, PermissionViewModel>.NewConfig().MapToConstructor(true);

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        _canEditRoleClaims = await AuthService.HasPermissionAsync(state.User, TravaloudAction.Update, TravaloudResource.RoleClaims);
        _canSearchRoleClaims = await AuthService.HasPermissionAsync(state.User, TravaloudAction.View, TravaloudResource.RoleClaims);

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => RoleService.GetByIdWithPermissionsAsync(Id), Snackbar,
                Logger)
            is {Permissions: not null} role)
        {
            _title = string.Format(L["{0} Permissions"], role.Name);
            _description = string.Format(L["Manage {0} Permissions"], role.Name);

            var permissions = state.User.GetTenant() == MultitenancyConstants.Root.Id
                ? TravaloudPermissions.All
                : TravaloudPermissions.Admin;

            _groupedRoleClaims = permissions
                .GroupBy(p => p.Resource)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    var permission = p.Adapt<PermissionViewModel>();
                    permission.Enabled = role.Permissions.Contains(permission.Name);
                    return permission;
                }).ToList());
        }

        _loaded = true;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        var allPermissions = _groupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var request = new UpdateRolePermissionsRequest()
        {
            RoleId = Id,
            Permissions = selectedPermissions.Where(x => x.Enabled).Select(x => x.Name).ToList(),
        };

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => RoleService.UpdatePermissionsAsync(request),
                Snackbar,Logger,
                successMessage: L["Updated Permissions."])
            is not null)
        {
            NavigationManager.NavigateTo("/permissions");
        }
    }

    private bool Search(PermissionViewModel permission) =>
        string.IsNullOrWhiteSpace(_searchString)
            || permission.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permission.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}

public record PermissionViewModel : TravaloudPermission
{
    public bool Enabled { get; set; }

    public PermissionViewModel(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
        : base(Description, Action, Resource, IsBasic, IsRoot)
    {
    }
}