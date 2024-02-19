using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Queries;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.PageManagement;

public partial class PageModals
{
    [Inject] protected IPageModalsService PageModalsService { get; set; } = default!;
    [Inject] protected IPagesService PagesService { get; set; } = default!;

    private EntityServerTableContext<PageModalDto, Guid, UpdatePageModalRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<PageModalDto, Guid, UpdatePageModalRequest> _table = default!;
    
    private ICollection<PageModalPageRequest>? Pages { get; set; }

    private MudSelect<PageModalPageRequest> _pagesSelect = null!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<PageModalDto, Guid, UpdatePageModalRequest>(
            entityName: L["Page Modal"],
            entityNamePlural: L["Page Modals"],
            entityResource: TravaloudResource.Pages,
            fields:
            [
                new EntityField<PageModalDto>(page => page.Title, L["Title"], "Title"),
                new EntityField<PageModalDto>(page => page.StartDate, L["From Date"], "StartDate"),
                new EntityField<PageModalDto>(page => page.EndDate, L["To Date"], "EndDate")
            ],
            enableAdvancedSearch: false,
            idFunc: partner => partner.Id,
            getDefaultsFunc: async () =>
            {
                var pages = await PagesService.SearchAsync(new SearchPagesRequest()
                {
                    PageNumber = 1,
                    PageSize = 99999
                });

                Pages = pages.Data.Select(x => new PageModalPageRequest()
                {
                    PageId = x.Id,
                    PageName = x.Title
                }).ToArray();

                return new UpdatePageModalRequest();
            },
            getDetailsFunc: async (id) =>
            {
                var pageModal = await PageModalsService.GetAsync(id);
                var pages = await PagesService.SearchAsync(new SearchPagesRequest()
                {
                    PageNumber = 1,
                    PageSize = 99999
                });

                Pages = pages.Data.Select(x => new PageModalPageRequest()
                {
                    PageId = x.Id,
                    PageName = x.Title
                }).ToArray();

                var adaptedPageModal = pageModal.Adapt<UpdatePageModalRequest>();
                adaptedPageModal.StartEndDateRange = new DateRange(pageModal.StartDate, pageModal.EndDate);
                
                if (pageModal.PageModalLookups.Any())
                    adaptedPageModal.SelectedPages = pageModal.PageModalLookups.Select(x => new PageModalPageRequest()
                    {
                        Id = x.Id,
                        PageId = x.PageId,
                        PageName = x.Title
                    }).ToList();
                
                return adaptedPageModal;
            },
            searchFunc: async filter => (await PageModalsService.SearchAsync(filter.Adapt<SearchPageModalsRequest>())).Adapt<PaginationResponse<PageModalDto>>(),
            createFunc: async service => await PageModalsService.CreateAsync(service.Adapt<CreatePageModalRequest>()),
            updateFunc: async (id, service) => await PageModalsService.UpdateAsync(id, service.Adapt<UpdatePageModalRequest>()),
            exportAction: string.Empty,
            deleteFunc: async id => await PageModalsService.DeleteAsync(id)
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
    
    private void SelectedValuesChanged(IEnumerable<PageModalPageRequest> values)
    {
        if (Context.AddEditModal != null) Context.AddEditModal.RequestModel.SelectedPages = values;
        _pagesSelect.Validate();
    }
}
