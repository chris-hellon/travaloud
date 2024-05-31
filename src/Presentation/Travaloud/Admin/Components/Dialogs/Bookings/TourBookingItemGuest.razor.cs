using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs.Guests;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class TourBookingItemGuest
{
    [Inject] protected IUserService UserService { get; set; } = default!;
    [Parameter] [EditorRequired] public BookingItemGuestRequest RequestModel { get; set; } = default!;
    [Parameter] public UpdateBookingItemRequest TourBookingItem { get; set; } = default!;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    private MudAutocomplete<UserDetailsDto> _additionalGuestsList = default!;
    [Parameter] public string GuestToUpdate { get; set; }
    
    public EditForm EditForm { get; set; } = default!;
    private FluentValidationValidator? _fluentValidationValidator;

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    async () =>
                    {
                        TourBookingItem.Guests ??= new List<BookingItemGuestRequest>();

                        if (!string.IsNullOrEmpty(GuestToUpdate))
                        {
                            var guestToRemove = TourBookingItem.Guests.FirstOrDefault(x => x.GuestId == GuestToUpdate);
                            
                            if (guestToRemove != null)
                                TourBookingItem.Guests.Remove(guestToRemove);
                        }

                        if (RequestModel.Guest != null)
                        {
                            var guest = await UserService.GetAsync(RequestModel.Guest.Id, CancellationToken.None);

                            var guestModel = new BookingItemGuestRequest()
                            {
                                GuestId = guest.Id.ToString(),
                                FirstName = guest.FirstName,
                                LastName = guest.LastName,
                                EmailAddress = guest.Email
                            };

                            if (TourBookingItem.Guests.Count > 0)
                            {
                                var lastItem = TourBookingItem.Guests.Last();
                                if (lastItem?.BookingItemId != null)
                                    TourBookingItem.Guests.Add(guestModel);
                            }
                            else
                            {
                                TourBookingItem.Guests.Add(guestModel);
                            }
                        }
                    },
                    Snackbar,
                    Logger,
                    $"Guest Added."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }

        await LoadingService.ToggleLoaderVisibility(false);
    }

    private async Task<IEnumerable<UserDetailsDto>> SearchAdditionalGuests(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Array.Empty<UserDetailsDto>();

        var guests = await UserService.SearchByDapperAsync(new SearchByDapperRequest()
        {
            Keyword = value,
            Role = "Guest",
            PageNumber = 1,
            PageSize = 99999,
            TenantId = MultiTenantContextAccessor.MultiTenantContext.TenantInfo.Id
        }, token);

        if (guests.Data.Count != 0) return guests.Data;
        
        return Array.Empty<UserDetailsDto>();
    }

    private async Task InvokeCreateGuestDialog()
    {
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(CreateUpdateGuest.TitleLabel), "Additional"}
        };

        var dialog = await DialogService.ShowAsync<CreateUpdateGuest>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var guestId = result.Data.ToString();
            var guest = await UserService.GetAsync(guestId, CancellationToken.None);
            
            RequestModel.Guest = guest;
            RequestModel.GuestId = guestId;
        }
    }

    private string GetUserDetailsLabel(UserDetailsDto e)
    {
        var details = new List<string>
        {
            $"{e.FirstName} {e.LastName}"
        };

        if (e.DateOfBirth.HasValue)
        {
            details.Add(e.DateOfBirth.Value.ToShortDateString());
        }

        if (!string.IsNullOrEmpty(e.Gender))
        {
            details.Add(e.Gender.GenderMatch());
        }

        if (!string.IsNullOrEmpty(e.Nationality))
        {
            details.Add(e.Nationality.TwoLetterCodeToCountry());
        }

        if (e.Email != null) details.Add(e.Email);

        return string.Join(" - ", details);
    }
}