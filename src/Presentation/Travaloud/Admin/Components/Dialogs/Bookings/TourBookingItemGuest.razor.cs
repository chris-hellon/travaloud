using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class TourBookingItemGuest
{
    [Parameter] public IList<UserDetailsDto>? Guests { get; set; }
    [Parameter] [EditorRequired] public BookingItemGuestRequest RequestModel { get; set; } = default!;
    [Parameter] public UpdateBookingItemRequest TourBookingItem { get; set; } = default!;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

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
                    () =>
                    {
                        TourBookingItem.Guests ??= new List<BookingItemGuestRequest>();

                        var guest = Guests?.FirstOrDefault(x => x.Id == RequestModel.Guest.Id);

                        if (guest == null) return Task.CompletedTask;
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
                            if (lastItem?.BookingItemId == null) return Task.CompletedTask;
                        
                            TourBookingItem.Guests.Add(guestModel);
                            
                            TourBookingItem.GuestQuantity ??= 1;
                            TourBookingItem.GuestQuantity++;
                        }
                        else
                        {
                            TourBookingItem.Guests.Add(guestModel);

                            TourBookingItem.GuestQuantity ??= 1;
                            TourBookingItem.GuestQuantity++;
                        }


                        return Task.CompletedTask;
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
    
    private async Task<IEnumerable<UserDetailsDto>> SearchGuests(string value)
    {
        await Task.Delay(5);

        return (string.IsNullOrEmpty(value) ? Guests : Guests .Where(x => (x.FirstName.Contains(value, StringComparison.InvariantCultureIgnoreCase) && x.LastName.Contains(value, StringComparison.InvariantCultureIgnoreCase)) || x.FirstName.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.LastName.Contains(value, StringComparison.InvariantCultureIgnoreCase))) ?? Array.Empty<UserDetailsDto>();
    }
}