using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Staff;

public partial class Staff
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    private EntityClientTableContext<UserDetailsDto, Guid, CreateUserRequest> Context { get; set; } = default!;

    private bool _canExportUsers;
    private bool _canViewRoles;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var authStateUser = authState.User;
        
        _canExportUsers = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.Export, TravaloudResource.Users);
        _canViewRoles = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.View, TravaloudResource.UserRoles);

        Context = new EntityClientTableContext<UserDetailsDto, Guid, CreateUserRequest>(
            entityName: L["Staff"],
            entityNamePlural: L["Staff"],
            entityResource: TravaloudResource.Users,
            searchAction: TravaloudAction.View,
            fields:
            [
                new EntityField<UserDetailsDto>(user => user.FirstName, L["First Name"]),
                new EntityField<UserDetailsDto>(user => user.LastName, L["Last Name"]),
                new EntityField<UserDetailsDto>(user => user.UserName, L["UserName"]),
                new EntityField<UserDetailsDto>(user => user.Email, L["Email"]),
                new EntityField<UserDetailsDto>(user => user.PhoneNumber, L["PhoneNumber"]),
                // new EntityField<UserDetailsDto>(user => user.EmailConfirmed, L["Email Confirmation"], Type: typeof(bool)),
                new EntityField<UserDetailsDto>(user => user.IsActive, L["Active"], Type: typeof(bool))
            ],
            idFunc: user => user.Id,
            loadDataFunc: async () => (await UserService.GetListAsync()).ToList(),
            searchFunc: (searchString, user) =>
                string.IsNullOrWhiteSpace(searchString)
                    || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: user => UserService.CreateAsync(user, GetOriginFromRequest(), TravaloudRoles.Basic),
            hasExtraActionsFunc: () => true,
            exportAction: string.Empty,
            updateAction: string.Empty,
            deleteAction: string.Empty,
            viewAction: string.Empty);
    }

    private void ViewProfile(in Guid userId) =>
        NavigationManager.NavigateTo($"/staff/{userId}/profile");

    private void ManageRoles(in Guid userId) =>
        NavigationManager.NavigateTo($"/staff/{userId}/roles");

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

        Context.AddEditModal?.ForceRender();
    }

    private string GetOriginFromRequest() => NavigationManager.BaseUri;
}