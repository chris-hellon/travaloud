using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Commands;
using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Enquiries;

public partial class ServicesEnquiries
{
    [Inject] protected IServicesEnquiriesService ServicesEnquiriesService { get; set; } = default!;

    private EntityServerTableContext<ServiceEnquiryDto, Guid, UpdateServiceEnquiryRequest> Context { get; set; } = default!;

    protected ServiceDto Service { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<ServiceEnquiryDto, Guid, UpdateServiceEnquiryRequest> _table = default!;
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<ServiceEnquiryDto, Guid, UpdateServiceEnquiryRequest>(
            entityName: L["Service Enquiry"],
            entityNamePlural: L["Service Enquiries"],
            entityResource: TravaloudResource.Enquiries,
            fields:
            [
                new EntityField<ServiceEnquiryDto>(enquiry => enquiry.Service.Title, L["Service"], "Service.Title"),
                new EntityField<ServiceEnquiryDto>(enquiry => enquiry.CreatedOn, L["Submit Date"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: enquiry => enquiry.Id,
            getDetailsFunc: async (id) =>
            {
                var serviceEnquiry = await ServicesEnquiriesService.GetAsync(id);
                Service = serviceEnquiry?.Service!;

                return serviceEnquiry.Adapt<UpdateServiceEnquiryRequest>();
            },
            searchFunc: async filter => (await ServicesEnquiriesService
                    .SearchAsync(filter.Adapt<SearchServiceEnquiriesRequest>()))
                .Adapt<PaginationResponse<ServiceEnquiryDto>>(),
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