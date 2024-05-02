using System.Globalization;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudExtensions;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.Bookings.Wizard;

public partial class SelectPrimaryGuest : ComponentBase
{
    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] public required TourBookingViewModel RequestModel { get; set; }
    
    [CascadingParameter] private MudStepper? _stepper { get; set; }
    
    private EntityServerTableContext<UserDetailsDto, string, UpdateUserRequest> Context { get; set; } = default!;
    
    private Dictionary<string, string>? _nationalities;

    private CultureInfo? _culture;
    
    protected override async Task OnInitializedAsync()
    {
        _culture = CultureInfo.GetCultureInfo("en-GB");
        _nationalities = Infrastructure.Common.Services.ListHelpers.GetNationalities();
        
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
            ],
            idFunc: user => user.Id,
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
                
                var guestId = await UserService.CreateAsync(adaptedUser, GetOriginFromRequest(), TravaloudRoles.Guest);
                
                var guest = await UserService.GetAsync(guestId, CancellationToken.None);
            
                RequestModel.Guest = guest;
                RequestModel.GuestId = guestId;
            },
            updateFunc: async (id, guest) => await UserService.UpdateAsync(guest, id.ToString()),
            exportAction: string.Empty);
    }
    
    private string GetOriginFromRequest() => NavigationManager.BaseUri;

    private async Task<IEnumerable<string>> SearchNationalities(string value)
    {
        await Task.Delay(5);

        return (string.IsNullOrEmpty(value) ? _nationalities?.Values : _nationalities?.Values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))) ?? Array.Empty<string>();
    }

    private async Task AddGuest(string guestId)
    {
        var guest = await UserService.GetAsync(guestId, CancellationToken.None);
        RequestModel.GuestId = guestId;
        RequestModel.Guest = guest;

        await _stepper.CompleteStep(0);
        await _stepper.SkipStep(0);
    }
}