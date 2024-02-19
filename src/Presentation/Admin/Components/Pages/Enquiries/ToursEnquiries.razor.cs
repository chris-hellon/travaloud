using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourEnquiries.Commands;
using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Enquiries;

public partial class ToursEnquiries
{
    [Inject] protected ITourEnquiriesService TourEnquiriesService { get; set; } = default!;

    private EntityServerTableContext<TourEnquiryDto, Guid, UpdateTourEnquiryRequest> Context { get; set; } = default!;

    protected TourDto Tour { get; set; } = default!;

    private EditContext? _editContext { get; set; }

    private EntityTable<TourEnquiryDto, Guid, UpdateTourEnquiryRequest> _table = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<TourEnquiryDto, Guid, UpdateTourEnquiryRequest>(
            entityName: L["Tour Enquiry"],
            entityNamePlural: L["Tour Enquiries"],
            entityResource: TravaloudResource.Enquiries,
            fields:
            [
                new EntityField<TourEnquiryDto>(enquiry => enquiry.Tour.Name, L["Tour"], "Tour.Name"),
                new EntityField<TourEnquiryDto>(enquiry => enquiry.Name, L["Name"], "Name"),
                new EntityField<TourEnquiryDto>(enquiry => enquiry.Email, L["Email Address"], "Email"),
                new EntityField<TourEnquiryDto>(enquiry => enquiry.ContactNumber, L["Contact Number"], "ContactNumber"),
                new EntityField<TourEnquiryDto>(enquiry => enquiry.CreatedOn, L["Submit Date"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: enquiry => enquiry.Id,
            getDetailsFunc: async (id) =>
            {
                var tourEnquiry = await TourEnquiriesService.GetAsync(id);
                Tour = tourEnquiry?.Tour!;

                return tourEnquiry.Adapt<UpdateTourEnquiryRequest>();
            },
            searchFunc: async filter => (await TourEnquiriesService
                    .SearchAsync(filter.Adapt<SearchTourEnquiriesRequest>()))
                .Adapt<PaginationResponse<TourEnquiryDto>>(),
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