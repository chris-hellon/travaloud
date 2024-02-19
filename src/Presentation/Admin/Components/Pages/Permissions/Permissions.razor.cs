using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Identity.Roles;
using Travaloud.Infrastructure.Auth;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Permissions;

public partial class Permissions
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject] private IRoleService RoleService { get; set; } = default!;

    private EntityClientTableContext<RoleDto, string?, CreateOrUpdateRoleRequest> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, TravaloudAction.View, TravaloudResource.RoleClaims);

        Context = new EntityClientTableContext<RoleDto, string?, CreateOrUpdateRoleRequest>(
            entityName: L["Permissions"],
            entityNamePlural: L["Permissions"],
            entityResource: TravaloudResource.Roles,
            searchAction: TravaloudAction.View,
            fields:
            [
                new EntityField<RoleDto>(role => role.Name, L["Name"]),
                new EntityField<RoleDto>(role => role.Description, L["Description"])
            ],
            idFunc: role => role.Id,
            loadDataFunc: async () => (await RoleService.GetListAsync()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RoleService.CreateOrUpdateAsync(role),
            updateFunc: async (_, role) => await RoleService.CreateOrUpdateAsync(role),
            deleteFunc: async id =>
            {
                if (id != null) await RoleService.DeleteAsync(id);
            },
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !TravaloudRoles.IsDefault(e.Name),
            canDeleteEntityFunc: e => !TravaloudRoles.IsDefault(e.Name),
            exportAction: string.Empty);
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        NavigationManager.NavigateTo($"/permissions/{roleId}");
    }
}