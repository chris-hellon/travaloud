using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class Services
{
    [Inject] protected IServicesService ServicesService { get; set; } = default!;

    private EntityServerTableContext<ServiceDto, Guid, UpdateServiceRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<ServiceDto, Guid, UpdateServiceRequest> _table = default!;

    private static Dictionary<string, bool> _wizardSteps => new()
    {
        {"Basic Information", true},
        {"Fields", true}
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<ServiceDto, Guid, UpdateServiceRequest>(
            entityName: L["Service"],
            entityNamePlural: L["Services"],
            entityResource: TravaloudResource.Services,
            fields: [new EntityField<ServiceDto>(service => service.Title, L["Title"], "Title")],
            enableAdvancedSearch: false,
            idFunc: service => service.Id,
            getDetailsFunc: async (id) =>
            {
                var service = await ServicesService.GetAsync(id);
                service.ServiceFields = service.ServiceFields.OrderBy(x => x.SortOrder).ToList();

                return service.Adapt<UpdateServiceRequest>();
            },
            searchFunc: async filter => (await ServicesService
                    .SearchAsync(filter.Adapt<SearchServiceRequest>()))
                .Adapt<PaginationResponse<ServiceDto>>(),
            createFunc: async service => await ServicesService.CreateAsync(service.Adapt<CreateServiceRequest>()),
            updateFunc: async (id, service) =>
                await ServicesService.UpdateAsync(id, service.Adapt<UpdateServiceRequest>()),
            exportAction: string.Empty,
            deleteFunc: async id => await ServicesService.DeleteAsync(id)
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

    public async Task InvokeFieldDialog(UpdateServiceFieldRequest requestModel, UpdateServiceRequest service,
        bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<UpdateServiceFieldRequest>>(JsonSerializer.Serialize(service.ServiceFields)) ?? new List<UpdateServiceFieldRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.WebsiteManagement.Services.ServiceField.RequestModel), requestModel},
            {nameof(Dialogs.WebsiteManagement.Services.ServiceField.Service), service},
            {nameof(Dialogs.WebsiteManagement.Services.ServiceField.Context), Context},
            {nameof(Dialogs.WebsiteManagement.Services.ServiceField.Id), isCreate ? null : requestModel.Id}
        };

        var dialog =
            await DialogService.ShowAsync<Dialogs.WebsiteManagement.Services.ServiceField>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            service.ServiceFields = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveField(UpdateServiceRequest service, Guid id)
    {
        string deleteContent =
            L[
                "You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Field", id)}
        };

        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            if (service.ServiceFields != null)
            {
                var serviceField = service.ServiceFields.FirstOrDefault(x => x.Id == id);
                if (serviceField != null)
                    service.ServiceFields.Remove(serviceField);
            }

            Context.AddEditModal?.ForceRender();
        }
    }
}