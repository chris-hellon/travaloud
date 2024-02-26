using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Guests;

public partial class GuestProfile
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject] protected IUserService UsersService { get; set; } = default!;

    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }

    private bool _active;
    private bool _emailConfirmed;
    private char _firstLetterOfName;
    private string? _firstName;
    private string? _lastName;
    private string? _phoneNumber;
    private string? _email;
    private string? _address;
    private string? _city;
    private string? _zipPostalCode;
    private string? _imageUrl;
    private DateTime? _dateOfBirth;
    private string? _gender;
    private string? _nationality;
    private string? _passportNumber;
    private string? _passportIssuingCountry;
    private DateTime? _passportIssueDate;
    private DateTime? _passportExpriryDate;
    private DateTime? _visaIssueDate;
    private DateTime? _visaExpiryDate;
    private bool _loaded;
    private bool _canToggleUserStatus;

    private CultureInfo? _culture;

    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequest {ActivateUser = _active, UserId = Id};
        await ServiceHelper.ExecuteCallGuardedAsync(() => UsersService.ToggleStatusAsync(request), Snackbar, Logger);
        NavigationManager.NavigateTo("/guests");
    }

    [Parameter]
    public string? ImageUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _culture = CultureInfo.GetCultureInfo("en-GB");

        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => UsersService.GetAsync(Id!), Snackbar, Logger)
            is { } user)
        {
            _firstName = user.FirstName;
            _lastName = user.LastName;
            _email = user.Email;
            _phoneNumber = user.PhoneNumber;
            _active = user.IsActive;
            _emailConfirmed = user.EmailConfirmed;
            _imageUrl = string.IsNullOrEmpty(user.ImageUrl) ? string.Empty : user.ImageUrl;
            _address = user.Address;
            _zipPostalCode = user.ZipPostalCode;
            _city = user.City;
            _dateOfBirth = user.DateOfBirth;
            _gender = user.Gender;
            _nationality = user.Nationality;
            _passportNumber = user.PassportNumber;
            _passportExpriryDate = user.PassportExpiryDate;
            _passportIssuingCountry = user.PassportIssuingCountry;
            _passportIssueDate = user.PassportIssueDate;
            _visaIssueDate = user.VisaIssueDate;
            _visaExpiryDate = user.VisaExpiryDate;
            Title = $"{_firstName} {_lastName}'s {_localizer["Profile"]}";
            Description = _email;
            if (_firstName?.Length > 0)
            {
                _firstLetterOfName = _firstName.ToUpper().FirstOrDefault();
            }
        }

        var state = await AuthState.GetAuthenticationStateAsync();
        _canToggleUserStatus = await AuthService.HasPermissionAsync(state.User, TravaloudAction.Update, TravaloudResource.Guests);
        _loaded = true;
    }
}