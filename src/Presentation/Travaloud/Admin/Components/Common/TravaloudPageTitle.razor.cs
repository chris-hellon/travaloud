using Microsoft.AspNetCore.Components;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Admin.Components.Common;

public partial class TravaloudPageTitle
{
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Description { get; set; }
    [Parameter] public RenderFragment? AdditionalContent { get; set; }

    private string PageTitle => $"{Title}{(TenantInfo != null ? $" - {TenantInfo.Name}" : "")}";
}