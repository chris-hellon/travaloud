using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using Travaloud.Application.Multitenancy;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Admin.Components.Pages;

public partial class Settings
{
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    private TenantViewModel TenantRequest { get; set; }

    protected override async Task OnInitializedAsync()
    {
        TenantRequest = TenantInfo.Adapt<TenantViewModel>();
    }
    
    private async Task UpdateTenantAsync()
    {
        
    }
}

public class TenantViewModel : UpdateTenantRequest
{
    public MudColor? MudPrimaryColor { get; set; }
    public MudColor? MudSecondaryColor { get; set; }
    public MudColor? MudTeritaryColor { get; set; }
}