using Mapster;
using Microsoft.AspNetCore.Components;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Seo;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.PageManagement;

public partial class PageRedirects : ComponentBase
{
    private EntityServerTableContext<SeoRedirectDto, DefaultIdType, UpdateSeoRedirectRequest> Context { get; set; } = default!;

    private EntityTable<SeoRedirectDto, DefaultIdType, UpdateSeoRedirectRequest> _table = default!;
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<SeoRedirectDto, DefaultIdType, UpdateSeoRedirectRequest>(
            entityName: L["Page Redirects"],
            entityNamePlural: L["Page Redirects"],
            entityResource: TravaloudResource.Seo,
            fields:
            [
                new EntityField<SeoRedirectDto>(page => page.Url, L["Url"], "Url"),
                new EntityField<SeoRedirectDto>(page => page.RedirectUrl, L["Redirect Url"], "RedirectUrl")
            ],
            enableAdvancedSearch: false,
            idFunc: partner => partner.Id,
            getDetailsFunc: async (id) =>
            {
                var partner = await PagesService.GetSeoRedirect(new GetSeoRedirectRequest(id));

                return partner.Adapt<UpdateSeoRedirectRequest>();
            },
            searchFunc: async filter => (await PagesService.GetSeoRedirects(filter.Adapt<GetSeoRedirectsRequest>())).Adapt<PaginationResponse<SeoRedirectDto>>(),
            createFunc: async service => await PagesService.CreateSeoRedirectAsync(service.Adapt<CreateSeoRedirectRequest>()),
            updateFunc: async (id, service) => await PagesService.UpdateSeoRedirectAsync(service.Adapt<UpdateSeoRedirectRequest>()),
            exportAction: string.Empty,
            deleteFunc: async id => await PagesService.DeleteSeoRedirectAsync(new DeleteSeoRedirectRequest(id))
        );
    }
}