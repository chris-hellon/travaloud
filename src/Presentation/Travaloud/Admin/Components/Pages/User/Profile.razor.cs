using Blazored.FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Application.Identity;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Identity;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.User;

public partial class Profile
{
    [Inject] protected IAuthenticationService AuthService { get; set; } = default!;
    [Inject] protected IPersonalService PersonalService { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    
    private readonly UpdateUserRequest _profileModel = new();

    private string? _imageUrl;
    private string? _userId;
    private char _firstLetterOfName;

    private FluentValidationValidator? _fluentValidationValidator;

    protected override async Task OnInitializedAsync()
    {
        if ((await AuthState.GetAuthenticationStateAsync()).User is { } userClaims)
        {
            _userId = userClaims.GetUserId();

            if (_userId != null)
            {
                var user = await UserManager.FindByIdAsync(_userId);
                _profileModel.Email = user?.Email;
                _profileModel.FirstName = user?.FirstName;
                _profileModel.LastName = user?.LastName;
                _profileModel.PhoneNumber = user?.PhoneNumber;
                _imageUrl = user?.ProfileImageUrl;
                _profileModel.Id = _userId;
            }
        }

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName.ToUpper().FirstOrDefault();
        }
    }

    private async Task UpdateProfileAsync()
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => PersonalService.UpdateProfileAsync(_profileModel), Snackbar, Logger))
        {
            Snackbar.Add(L["Your Profile has been updated."], Severity.Success);
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var fileName = $"{_userId}-{DefaultIdType.NewGuid():N}";
        fileName = fileName[..Math.Min(fileName.Length, 90)];
       
        var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

        if (fileUploadDetails != null)
        {
            _profileModel.Image = new FileUploadRequest() {Name = fileName, Data = fileUploadDetails.FileInBytes, Extension = fileUploadDetails.Extension};
            await UpdateProfileAsync();
        }
    }

    public async Task RemoveImageAsync()
    {
        string deleteContent = L["You're sure you want to delete your Profile Image?"];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), deleteContent}
        };
        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            _profileModel.DeleteCurrentImage = true;
            await UpdateProfileAsync();
        }
    }
}