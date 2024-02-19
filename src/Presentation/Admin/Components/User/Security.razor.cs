using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Application.Identity;
using Travaloud.Application.Identity.Users.Password;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.User;

public partial class Security
{
    [Inject] public IPersonalService PersonalService { get; set; } = default!;

    private readonly ChangePasswordRequest _passwordModel = new();

    private FluentValidationValidator? _fluentValidationValidator;

    private async Task ChangePasswordAsync()
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => PersonalService.ChangePasswordAsync(_passwordModel),
                Snackbar,
                Logger,
                L["Password Changed!"]))
        {
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
        }
    }

    private bool _currentPasswordVisibility;
    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
        else
        {
            if (_currentPasswordVisibility)
            {
                _currentPasswordVisibility = false;
                _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _currentPasswordInput = InputType.Password;
            }
            else
            {
                _currentPasswordVisibility = true;
                _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                _currentPasswordInput = InputType.Text;
            }
        }
    }
}