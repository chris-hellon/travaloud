using System.Globalization;
using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Pages.Bookings;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class CreateGuest : ComponentBase
{
    [Parameter] public bool EmailRequired { get; set; }
    [Parameter] public required string TitleLabel { get; set; }
    // [Parameter] public BookingItemGuestRequest? BookingItem { get; set; }
    // [Parameter] public TourBookingViewModel? TourBooking { get; set; }
    
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    
    [Inject] private IUserService UserService { get; set; }
    
    public UpdateUserRequest RequestModel { get; set; } = new();
    
    private FluentValidationValidator? _fluentValidationValidator;

    public EditForm EditForm { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private CultureInfo? _culture;

    private Dictionary<string, string>? _nationalities;

    protected override Task OnParametersSetAsync()
    {
        _culture = CultureInfo.GetCultureInfo("en-GB");
        _nationalities = ListHelpers.GetNationalities();
        EditContext = new EditContext(RequestModel);
        RequestModel.EmailRequired = EmailRequired;
        
        return base.OnParametersSetAsync();
    }

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (!EmailRequired)
        {
            RequestModel.Email = $"{Guid.NewGuid()}@travaloudguest.com";
        }

        if (await _fluentValidationValidator!.ValidateAsync())
        {
            var adaptedUser = RequestModel.Adapt<CreateUserRequest>();
            adaptedUser.IsGuest = true;
            
            var guestId = await ServiceHelper.ExecuteCallGuardedAsync(() =>
                    UserService.CreateAsync(adaptedUser, TravaloudRoles.Guest),
                Snackbar,
                Logger,
                "Guest Added");

            if (guestId == "Validation Errors Occurred.")
                Snackbar.Add("One or more validation errors occurred.");
            else
                MudDialog.Close(DialogResult.Ok(guestId));
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }
        
        await LoadingService.ToggleLoaderVisibility(false);
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private string GetOriginFromRequest() => NavigationManager.BaseUri;
    
    private async Task<IEnumerable<string>> SearchNationalities(string value)
    {
        await Task.Delay(5);

        return (string.IsNullOrEmpty(value) ? _nationalities?.Values : _nationalities?.Values.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))) ?? Array.Empty<string>();
    }
}