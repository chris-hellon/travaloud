using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Application.Catalog.Seo;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.PageManagement;

public partial class GlobalSeo : ComponentBase
{
    private UpdateSeoRequest _inputModel = new();

    protected override async Task OnInitializedAsync()
    {
        var seoDetails = await PagesService.GetSeoAsync(new GetSeoRequest());

        if (seoDetails != null)
            _inputModel = seoDetails.Adapt<UpdateSeoRequest>();
    }

    private async Task UpdateSeoAsync()
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => PagesService.UpdateSeoAsync(_inputModel), Snackbar, Logger))
        {
            Snackbar.Add(L["Global SEO has been updated."], Severity.Success);
        }
    }
}