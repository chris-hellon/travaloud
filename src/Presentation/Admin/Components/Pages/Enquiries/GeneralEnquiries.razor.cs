using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Enquiries.Commands;
using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Enquiries;

public partial class GeneralEnquiries
{
    [Inject] protected IGeneralEnquiriesService GeneralEnquiriesService { get; set; } = default!;

    private EntityServerTableContext<GeneralEnquiryDto, Guid, UpdateGeneralEnquiryRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<GeneralEnquiryDto, Guid, UpdateGeneralEnquiryRequest> _table = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<GeneralEnquiryDto, Guid, UpdateGeneralEnquiryRequest>(
            entityName: L["General Enquiry"],
            entityNamePlural: L["General Enquiries"],
            entityResource: TravaloudResource.Enquiries,
            fields:
            [
                new EntityField<GeneralEnquiryDto>(enquiry => enquiry.Name, L["Name"], "Name"),
                new EntityField<GeneralEnquiryDto>(enquiry => enquiry.Email, L["Email Address"], "Email"),
                new EntityField<GeneralEnquiryDto>(enquiry => enquiry.ContactNumber, L["Contact Number"], "ContactNumber"),
                new EntityField<GeneralEnquiryDto>(enquiry => enquiry.CreatedOn, L["Submit Date"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: enquiry => enquiry.Id,
            searchFunc: async filter => (await GeneralEnquiriesService
                    .SearchAsync(filter.Adapt<SearchGeneralEnquiriesRequest>()))
                .Adapt<PaginationResponse<GeneralEnquiryDto>>(),
            createAction: string.Empty,
            updateAction: string.Empty,
            exportAction: string.Empty,
            deleteAction: string.Empty
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