using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Application.Catalog.Partners.Queries;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Partners
{
    [Inject] protected IPartnersService PartnersService { get; set; } = default!;

    private EntityServerTableContext<PartnerDto, Guid, UpdatePartnerRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<PartnerDto, Guid, UpdatePartnerRequest> _table = default!;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        {"Basic Information", true},
        {"Additional Contacts", true},
    };

    private MudTable<UpdatePartnerContactRequest> PartnerContactsTable { get; set; } = default!;

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<PartnerDto, Guid, UpdatePartnerRequest>(
            entityName: L["Partner"],
            entityNamePlural: L["Partners"],
            entityResource: TravaloudResource.Partners,
            fields:
            [
                new EntityField<PartnerDto>(partner => partner.Name, L["Name"], "Name"),
                new EntityField<PartnerDto>(partner => partner.Address, L["Address"], "Address"),
                new EntityField<PartnerDto>(partner => partner.PrimaryContactName, L["Primary Contact Name"], "PrimaryContactName"),
                new EntityField<PartnerDto>(partner => partner.PrimaryEmailAddress, L["Primary Contact Email Address"], "PrimaryEmailAddress"),
                new EntityField<PartnerDto>(partner => partner.PrimaryContactNumber, L["Primary Contact Number"], "PrimaryContactNumber")
            ],
            enableAdvancedSearch: false,
            idFunc: partner => partner.Id,
            getDetailsFunc: async (id) =>
            {
                var partner = await PartnersService.GetAsync(id);

                return partner.Adapt<UpdatePartnerRequest>();
            },
            searchFunc: async filter => (await PartnersService.SearchAsync(filter.Adapt<SearchPartnersRequest>())).Adapt<PaginationResponse<PartnerDto>>(),
            createFunc: async service => await PartnersService.CreateAsync(service.Adapt<CreatePartnerRequest>()),
            updateFunc: async (id, service) => await PartnersService.UpdateAsync(id, service.Adapt<UpdatePartnerRequest>()),
            exportAction: string.Empty,
            deleteFunc: async id => await PartnersService.DeleteAsync(id)
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

    public async Task InvokeContactDialog(UpdatePartnerContactRequest requestModel, UpdatePartnerRequest partner,
        bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<UpdatePartnerContactRequest>>(JsonSerializer.Serialize(partner.PartnerContacts)) ?? new List<UpdatePartnerContactRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.Partners.PartnerContact.RequestModel), requestModel},
            {nameof(Dialogs.Partners.PartnerContact.Partner), partner},
            {nameof(Dialogs.Partners.PartnerContact.Context), Context},
            {nameof(Dialogs.Partners.PartnerContact.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<Components.Dialogs.Partners.PartnerContact>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            partner.PartnerContacts = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveContact(UpdatePartnerRequest partner, UpdatePartnerContactRequest request)
    {
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Field", request.Name)}
        };

        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            partner.PartnerContacts?.Remove(request);

            Context.AddEditModal?.ForceRender();
        }
    }
}