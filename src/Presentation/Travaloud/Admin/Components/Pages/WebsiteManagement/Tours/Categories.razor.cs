using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;

public partial class Categories
{
    [Inject] protected ITourCategoriesService TourCategoriesService { get; set; } = default!;

    private EntityServerTableContext<TourCategoryDto, DefaultIdType, TourCategoryViewModel> Context { get; set; } = default!;
    private EntityTable<TourCategoryDto, DefaultIdType, TourCategoryViewModel> _table = default!;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        {"Basic Information", true},
        {"Image", true}
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<TourCategoryDto, DefaultIdType, TourCategoryViewModel>(
            entityName: L["Tour Category"],
            entityNamePlural: L["Tour Categories"],
            entityResource: TravaloudResource.Tours,
            fields: [new EntityField<TourCategoryDto>(tour => tour.Name, L["Name"], "Name")],
            enableAdvancedSearch: false,
            idFunc: tour => tour.Id,
            searchFunc: async filter => (await TourCategoriesService
                    .SearchAsync(filter.Adapt<SearchTourCategoriesRequest>()))
                .Adapt<PaginationResponse<TourCategoryDto>>(),
            createFunc: async tour =>
            {
                if (!string.IsNullOrEmpty(tour.ImageInBytes))
                {
                    tour.Image = new FileUploadRequest()
                    {
                        Data = tour.ImageInBytes, Extension = tour.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }

                tour.TopLevelCategory = true;

                await TourCategoriesService.CreateAsync(tour.Adapt<CreateTourCategoryRequest>());
                tour.ImageInBytes = string.Empty;
            },
            getDetailsFunc: async (id) =>
            {
                var tour = await TourCategoriesService.GetAsync(id);
                return tour.Adapt<TourCategoryViewModel>();
            },
            updateFunc: async (id, tour) =>
            {
                if (!string.IsNullOrEmpty(tour.ImageInBytes))
                {
                    tour.DeleteCurrentImage = true;
                    tour.Image = new FileUploadRequest()
                    {
                        Data = tour.ImageInBytes, Extension = tour.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }

                await TourCategoriesService.UpdateAsync(id, tour.Adapt<UpdateTourCategoryRequest>());
                tour.ImageInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await TourCategoriesService.DeleteAsync(id)
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

    public void ClearImageInBytes()
    {
        if (Context.AddEditModal?.RequestModel == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentImageFlag()
    {
        if (Context.AddEditModal?.RequestModel == null) return;
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.ImagePath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
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
}

public class TourCategoryViewModel : UpdateTourCategoryRequest
{
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}