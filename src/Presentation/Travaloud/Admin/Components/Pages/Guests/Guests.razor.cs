using System.Globalization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.Guests;

public partial class Guests
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    private EntityClientTableContext<UserDetailsDto, Guid, UpdateUserRequest> Context { get; set; } = default!;

    private bool _canExportUsers;
    private bool _canEditGuest;

    private Dictionary<string, string>? _nationalities;

    private CultureInfo? _culture;

    protected override async Task OnInitializedAsync()
    {
        _culture = CultureInfo.GetCultureInfo("en-GB");
        _nationalities = Infrastructure.Common.Services.ListHelpers.GetNationalities();

        var authState = await AuthState.GetAuthenticationStateAsync();
        var authStateUser = authState.User;
        
        _canExportUsers = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.Export, TravaloudResource.Guests);
        _canEditGuest = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.Update, TravaloudResource.Guests);

        Context = new EntityClientTableContext<UserDetailsDto, Guid, UpdateUserRequest>(
            entityName: L["Guest"],
            entityNamePlural: L["Guests"],
            entityResource: TravaloudResource.Guests,
            deleteAction: string.Empty,
            fields:
            [
                new EntityField<UserDetailsDto>(user => user.FirstName, L["First Name"]),
                new EntityField<UserDetailsDto>(user => user.LastName, L["Last Name"]),
                new EntityField<UserDetailsDto>(user => user.UserName, L["UserName"]),
                new EntityField<UserDetailsDto>(user => user.Email, L["Email"]),
                new EntityField<UserDetailsDto>(user => user.PhoneNumber, L["PhoneNumber"]),
                // new EntityField<UserDetailsDto>(user => user.EmailConfirmed, L["Email Confirmation"], Type: typeof(bool)),
                // new EntityField<UserDetailsDto>(user => user.IsActive, L["Active"], Type: typeof(bool))
            ],
            idFunc: user => user.Id,
            loadDataFunc: async () => (await UserService.GetListAsync(TravaloudRoles.Guest)).ToList(),
            searchFunc: (searchString, user) =>
                string.IsNullOrWhiteSpace(searchString)
                || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            getDefaultsFunc: () => Task.FromResult(new UpdateUserRequest
            {
                IsGuest = true
            }),
            getDetailsFunc: async (id) =>
            {
                var guest = await UserService.GetAsync(id.ToString());
                var adaptedGuest = guest.Adapt<UpdateUserRequest>();
                adaptedGuest.Genders = adaptedGuest.Gender?.Split(",").ToHashSet();

                return adaptedGuest;
            },
            createFunc: async user =>
            {
                var adaptedUser = user.Adapt<CreateUserRequest>();
                adaptedUser.Password = MultitenancyConstants.DefaultPassword;
                adaptedUser.ConfirmPassword = MultitenancyConstants.DefaultPassword;
                
                await UserService.CreateAsync(adaptedUser, GetOriginFromRequest(), TravaloudRoles.Guest);
            },
            hasExtraActionsFunc: () => true,
            updateFunc: async (id, guest) => await UserService.UpdateAsync(guest, id.ToString()),
            exportAction: string.Empty);
    }

    private void ViewProfile(in Guid userId) =>
        NavigationManager.NavigateTo($"/guests/{userId}/profile");

    private void ManageRoles(in Guid userId) =>
        NavigationManager.NavigateTo($"/guests/{userId}/roles");

    private string GetOriginFromRequest() => NavigationManager.BaseUri;

    private async Task<IEnumerable<string>> SearchNationalities(string value)
    {
        await Task.Delay(5);

        return (string.IsNullOrEmpty(value) ? _nationalities?.Values : _nationalities?.Values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))) ?? Array.Empty<string>();
    }
}