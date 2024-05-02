using System.Globalization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.Guests;

public partial class Guests
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    private EntityServerTableContext<UserDetailsDto, string, UpdateUserRequest> Context { get; set; } = default!;

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

        Context = new EntityServerTableContext<UserDetailsDto, string, UpdateUserRequest>(
            entityName: L["Guest"],
            entityNamePlural: L["Guests"],
            entityResource: TravaloudResource.Guests,
            deleteAction: string.Empty,
            fields:
            [
                new EntityField<UserDetailsDto>(user => user.FullName, L["Name"], "FullName"),
                new EntityField<UserDetailsDto>(user => user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToShortDateString() : "-", L["Date Of Birth"], "DateOfBirth"),
                new EntityField<UserDetailsDto>(user => user.Nationality?.TwoLetterCodeToCountry(), L["Nationality"], "Nationality"),
                new EntityField<UserDetailsDto>(user => user.Gender?.GenderMatch(), L["Gender"], "Gender"),
                new EntityField<UserDetailsDto>(user => user.Email, L["Email"], "Email"),
                new EntityField<UserDetailsDto>(user => user.PhoneNumber, L["Phone Number"], "PhoneNumber"),
                // new EntityField<UserDetailsDto>(user => user.EmailConfirmed, L["Email Confirmation"], Type: typeof(bool)),
                // new EntityField<UserDetailsDto>(user => user.IsActive, L["Active"], Type: typeof(bool))
            ],
            idFunc: user => user.Id,
            // loadDataFunc: async () => (await UserService.GetListAsync(TravaloudRoles.Guest)).ToList(),
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<SearchByDapperRequest>();
                adaptedFilter.Role = "Guest";
                adaptedFilter.TenantId = MultiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;

                if (adaptedFilter.OrderBy is {Length: 0})
                    adaptedFilter.OrderBy =
                    [
                        "FullName Ascending"
                    ];
                
                var request = await UserService.SearchByDapperAsync(adaptedFilter, CancellationToken.None);

                return request.Adapt<PaginationResponse<UserDetailsDto>>();
            },
            // searchFunc: (searchString, user) => await UserService.SearchAsync()
            //     string.IsNullOrWhiteSpace(searchString)
            //     || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            //     || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            //     || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            //     || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            //     || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            getDefaultsFunc: () => Task.FromResult(new UpdateUserRequest
            {
                IsGuest = true
            }),
            getDetailsFunc: async (id) =>
            {
                var guest = await UserService.GetAsync(id.ToString());
                var adaptedGuest = guest.Adapt<UpdateUserRequest>();
                
                adaptedGuest.Gender = adaptedGuest.Gender.GenderMatch();
                adaptedGuest.Genders = adaptedGuest.Gender?.Split(",").ToHashSet();
                adaptedGuest.Nationality = adaptedGuest.Nationality?.TwoLetterCodeToCountry();
                
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