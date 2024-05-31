using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.PageManagement;

public partial class Pages
{
    [Inject] protected IPagesService PagesService { get; set; } = default!;

    private EntityServerTableContext<PageDto, DefaultIdType, UpdatePageRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<PageDto, DefaultIdType, UpdatePageRequest> _table = default!;
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<PageDto, DefaultIdType, UpdatePageRequest>(
            entityName: L["Page"],
            entityNamePlural: L["Pages"],
            entityResource: TravaloudResource.Pages,
            fields:
            [
                new EntityField<PageDto>(page => page.Title, L["Title"], "Title")
            ],
            enableAdvancedSearch: false,
            idFunc: partner => partner.Id,
            getDetailsFunc: async (id) =>
            {
                var partner = await PagesService.GetAsync(id);

                return partner.Adapt<UpdatePageRequest>();
            },
            searchFunc: async filter => (await PagesService.SearchAsync(filter.Adapt<SearchPagesRequest>())).Adapt<PaginationResponse<PageDto>>(),
            // createFunc: async service => await PagesService.CreateAsync(service.Adapt<CreatePageRequest>()),
            updateFunc: async (id, service) => await PagesService.UpdateAsync(id, service.Adapt<UpdatePageRequest>()),
            exportAction: string.Empty,
            createAction: string.Empty,
            deleteFunc: async id => await PagesService.DeleteAsync(id)
        );
    }

    private string? _searchName;

    private string SearchName
    {
        get => _searchName ?? string.Empty;
        set
        {
            _searchName = value;
            _ = _table.ReloadDataAsync();
        }
    }
}