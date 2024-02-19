using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Multitenancy;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Tenants;

public partial class Tenants
{
    [Inject] private ITenantService TenantsService { get; set; } = default!;
    
    private string? _searchString;
    
    protected EntityClientTableContext<TenantDetail, string, UpdateTenantRequest> Context { get; set; } = default!;
    
    private List<TenantDetail> _tenants = new();

    private EntityTable<TenantDetail, string, UpdateTenantRequest> EntityTable { get; set; } = default!;
    
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canUpgrade;
    private bool _canModify;

    protected override async Task OnInitializedAsync()
    {
        Context = new EntityClientTableContext<TenantDetail, string, UpdateTenantRequest>(
            entityName: L["Tenant"],
            entityNamePlural: L["Tenants"],
            entityResource: TravaloudResource.Tenants,
            searchAction: TravaloudAction.View,
            deleteAction: string.Empty,
            fields:
            [
                new EntityField<TenantDetail>(tenant => tenant.Id, L["Id"]),
                new EntityField<TenantDetail>(tenant => tenant.Name, L["Name"]),
                new EntityField<TenantDetail>(tenant => tenant.AdminEmail, L["Admin Email"]),
                new EntityField<TenantDetail>(tenant => tenant.ValidUpto.ToString("MMM dd, yyyy"), L["Valid Upto"]),
                new EntityField<TenantDetail>(tenant => tenant.IsActive, L["Active"], Type: typeof(bool))
            ],
            idFunc: tenant => tenant.Id,
            getDetailsFunc: async (id) =>
            {
                var tenant = await TenantsService.GetByIdAsync(id);
                return tenant.Adapt<UpdateTenantRequest>();
            },
            loadDataFunc: async () => _tenants = (await TenantsService.GetAllAsync()).Adapt<List<TenantDetail>>(),
            searchFunc: (searchString, tenantDto) =>
                string.IsNullOrWhiteSpace(searchString)
                || tenantDto.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase),
            createFunc: tenant => TenantsService.CreateAsync(tenant.Adapt<CreateTenantRequest>(), new CancellationToken()),
            updateFunc: async (id, tenant) => await TenantsService.UpdateAsync(tenant.Adapt<UpdateTenantRequest>(), new CancellationToken()),
            hasExtraActionsFunc: () => true,
            exportAction: string.Empty);

        var state = await AuthState.GetAuthenticationStateAsync();
        _canUpgrade = await AuthService.HasPermissionAsync(state.User, TravaloudAction.UpgradeSubscription, TravaloudResource.Tenants);
        _canModify = await AuthService.HasPermissionAsync(state.User, TravaloudAction.Update, TravaloudResource.Tenants);
    }

    private void ViewTenantDetails(string id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        tenant.ShowDetails = !tenant.ShowDetails;
        foreach (var otherTenants in _tenants.Except(new[] {tenant}))
        {
            otherTenants.ShowDetails = false;
        }
    }

    private async Task ViewUpgradeSubscriptionModalAsync(string id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        var parameters = new DialogParameters
        {
            {
                nameof(UpgradeSubscriptionModal.Request),
                new UpgradeSubscriptionRequest
                {
                    TenantId = tenant.Id,
                    ExtendedExpiryDate = tenant.ValidUpto
                }
            }
        };
        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<UpgradeSubscriptionModal>(L["Upgrade Subscription"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task DeactivateTenantAsync(string id)
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => TenantsService.DeactivateAsync(id),
                Snackbar,
                Logger,
                L["Tenant Deactivated."]) is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task ActivateTenantAsync(string id)
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => TenantsService.ActivateAsync(id),
                Snackbar,
                Logger,
                L["Tenant Activated."]) is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    public class TenantDetail : TenantDto
    {
        public bool ShowDetails { get; set; }
    }
}