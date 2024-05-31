using System.Globalization;
using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Dialogs.Guests;

public partial class CreateUpdateGuest : ComponentBase
{
    [Parameter] public bool EmailRequired { get; set; }
    [Parameter] public string? TitleLabel { get; set; }
    [Parameter] public string? Id { get; set; }
    
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    
    [Inject] private IUserService UserService { get; set; }
    
    public UpdateUserRequest RequestModel { get; set; } = new();
    
    private FluentValidationValidator? _fluentValidationValidator;

    public EditForm EditForm { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private CultureInfo? _culture;

    private Dictionary<string, string>? _nationalities;

    protected override async Task OnParametersSetAsync()
    {
        _culture = CultureInfo.GetCultureInfo("en-GB");
        _nationalities = ListHelpers.GetNationalities();

        if (!string.IsNullOrEmpty(Id))
        {
            var guest = await UserService.GetAsync(Id, CancellationToken.None);
            RequestModel = guest.Adapt<UpdateUserRequest>();
        }
        
        EditContext = new EditContext(RequestModel);
        RequestModel.EmailRequired = EmailRequired;
    }

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var adaptedUser = RequestModel.Adapt<UpdateUserRequest>();
                adaptedUser.IsGuest = true;
                
                await ServiceHelper.ExecuteCallGuardedAsync(() =>
                        UserService.UpdateAsync(adaptedUser, Id),
                    Snackbar,
                    Logger,
                    "Guest Updated"); 
                
                MudDialog.Close(DialogResult.Ok($"{adaptedUser.FirstName} {adaptedUser.LastName}"));
            }
            else
            {
                var adaptedUser = RequestModel.Adapt<CreateUserRequest>();
                adaptedUser.IsGuest = true;
                adaptedUser.Username = DefaultIdType.NewGuid().ToString();
            
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