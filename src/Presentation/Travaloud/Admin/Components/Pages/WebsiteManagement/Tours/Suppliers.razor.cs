using System.Text;
using System.Text.Encodings.Web;
using BlazorTemplater;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Travaloud.Admin.Components.Account.Pages;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Identity;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;

public partial class Suppliers
{
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; } = default!;

    [Inject] protected IMailService MailService { get; set; } = default!;

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    private EntityServerTableContext<UserDetailsDto, string, CreateUserRequest> Context { get; set; } = default!;

    private bool _canExportUsers;
    private bool _canViewRoles;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var authStateUser = authState.User;
        
        _canExportUsers = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.Export, TravaloudResource.Users);
        _canViewRoles = await AuthService.HasPermissionAsync(authStateUser, TravaloudAction.View, TravaloudResource.UserRoles);

        Context = new EntityServerTableContext<UserDetailsDto, string, CreateUserRequest>(
            entityName: L["Supplier"],
            entityNamePlural: L["Suppliers"],
            entityResource: TravaloudResource.Suppliers,
            exportAction: string.Empty,
            updateAction: string.Empty,
            deleteAction: string.Empty,
            viewAction: string.Empty,
            fields:
            [
                new EntityField<UserDetailsDto>(user => user.FullName, L["Name"], "FullName"),
                new EntityField<UserDetailsDto>(user => user.Email, L["Email"], "Email"),
                new EntityField<UserDetailsDto>(user => user.PhoneNumber, L["Phone Number"], "PhoneNumber"),
                new EntityField<UserDetailsDto>(user => user.Address, L["Address"], "Address"),
                new EntityField<UserDetailsDto>(user => user.City, L["City"], "City")
            ],
            idFunc: user => user.Id,
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<SearchByDapperRequest>();
                adaptedFilter.Role = TravaloudRoles.Supplier;
                adaptedFilter.TenantId = MultiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;

                if (adaptedFilter.OrderBy is {Length: 0})
                    adaptedFilter.OrderBy =
                    [
                        "FullName Ascending"
                    ];
                
                var request = await UserService.SearchByDapperAsync(adaptedFilter, CancellationToken.None);

                return request.Adapt<PaginationResponse<UserDetailsDto>>();
            },
            getDefaultsFunc: () => Task.FromResult(new CreateUserRequest
            {
                IsGuest = false
            }),
            getDetailsFunc: async (id) =>
            {
                var guest = await UserService.GetAsync(id.ToString());
                var adaptedGuest = guest.Adapt<CreateUserRequest>();
                
                adaptedGuest.Gender = adaptedGuest.Gender.GenderMatch();
                adaptedGuest.Nationality = adaptedGuest.Nationality?.TwoLetterCodeToCountry();
                
                return adaptedGuest;
            },
            createFunc: async user =>
            {
                var adaptedUser = user.Adapt<CreateUserRequest>();
                await UserService.CreateAsync(adaptedUser, GetOriginFromRequest(), TravaloudRoles.Supplier);
            },
            hasExtraActionsFunc: () => true);
    }

    private void ViewProfile(in string userId) =>
        NavigationManager.NavigateTo($"/management/tours/suppliers/{userId}/profile");

    private void ManageRoles(in string userId) =>
        NavigationManager.NavigateTo($"/supplier/{userId}/roles");

    private async Task SendPasswordResetLink(string email)
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        var user = await UserManager.FindByEmailAsync(email);

        if (user is null)
            return;

        await ServiceHelper.ExecuteCallGuardedAsync(async () =>
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("account/reset-password").AbsoluteUri,
                new Dictionary<string, object?> {["code"] = code});
        
            var mailHtml = new ComponentRenderer<EmailTemplates.ForgotPasswordConfirmation>()
                .Set(c => c.Model, new ForgotPassword.PasswordResetModel(user, email, HtmlEncoder.Default.Encode(callbackUrl), TenantInfo!))
                .Render();
            
            var mailRequest = new MailRequest(
                [email],
                L["Reset Password"],
                mailHtml);

            await MailService.SendAsync(mailRequest);
        }, Snackbar, Logger, "Password Reset Sent Successfully.");
        
        await LoadingService.ToggleLoaderVisibility(false);
        StateHasChanged();
    }
    
    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }

        Context.AddEditModal?.ForceRender();
    }

    private string GetOriginFromRequest() => NavigationManager.BaseUri;
}