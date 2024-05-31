using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class Locations
{
    [Inject] protected IDestinationsService DestinationsService { get; set; } = default!;

    private EntityServerTableContext<DestinationDto, DefaultIdType, DestinationViewModel> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<DestinationDto, DefaultIdType, DestinationViewModel> _table = default!;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        { "Basic Information", true },
        { "Description", true },
        { "Additional Information",  true },
        { "Image",  true },
    };
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<DestinationDto, DefaultIdType, DestinationViewModel>(
            entityName: L["Location"],
            entityNamePlural: L["Locations"],
            entityResource: TravaloudResource.Locations,
            fields: [new EntityField<DestinationDto>(dest => dest.Name, L["Name"], "Name")],
            enableAdvancedSearch: false,
            idFunc: destination => destination.Id,
            searchFunc: async filter => (await DestinationsService
                    .SearchAsync(filter.Adapt<SearchDestinationsRequest>()))
                .Adapt<PaginationResponse<DestinationDto>>(),
            createFunc: async destination =>
            {
                if (!string.IsNullOrEmpty(destination.ImageInBytes))
                {
                    destination.Image = new FileUploadRequest()
                    {
                        Data = destination.ImageInBytes, Extension = destination.ImageExtension ?? string.Empty,
                        Name = $"{destination.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }

                await DestinationsService.CreateAsync(destination.Adapt<CreateDestinationRequest>());
                destination.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, destination) =>
            {
                if (!string.IsNullOrEmpty(destination.ImageInBytes))
                {
                    destination.DeleteCurrentImage = true;
                    destination.Image = new FileUploadRequest()
                    {
                        Data = destination.ImageInBytes, Extension = destination.ImageExtension ?? string.Empty,
                        Name = $"{destination.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }

                await DestinationsService.UpdateAsync(id, destination.Adapt<UpdateDestinationRequest>());
                destination.ImageInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await DestinationsService.DeleteAsync(id)
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

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                Context.AddEditModal.RequestModel.ImageExtension = fileUploadDetails.Extension;
                Context.AddEditModal.RequestModel.ImageInBytes = fileUploadDetails.FileInBytes;
                Context.AddEditModal.ForceRender();
            }
        }
    }

    public void ClearImageInBytes()
    {
        if (Context.AddEditModal == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentImageFlag()
    {
        if (Context.AddEditModal == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.ImagePath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }
}

public class DestinationViewModel : UpdateDestinationRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}