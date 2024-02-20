using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class TravelGuides
{
    [Inject] protected ITravelGuidesService TravelGuidesService { get; set; } = default!;

    private EntityServerTableContext<TravelGuideDto, Guid, TravelGuideViewModel> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<TravelGuideDto, Guid, TravelGuideViewModel> _table = default!;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        {"Basic Information", true},
        {"Description", true},
        {"SEO", true},
        {"Images", true},
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<TravelGuideDto, Guid, TravelGuideViewModel>(
            entityName: L["Travel Guide"],
            entityNamePlural: L["Travel Guides"],
            entityResource: TravaloudResource.TravelGuides,
            fields: [
                new EntityField<TravelGuideDto>(travelGuide => travelGuide.Title, L["Name"], "Name"),
                // new EntityField<TravelGuideDto>(travelGuide => travelGuide.CreatedBy, L["Created By"], "CreatedBy"),
                new EntityField<TravelGuideDto>(travelGuide => travelGuide.CreatedOn, L["Created On"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: destination => destination.Id,
            searchFunc: async filter => (await TravelGuidesService
                    .SearchAsync(filter.Adapt<SearchTravelGuidesRequest>()))
                .Adapt<PaginationResponse<TravelGuideDto>>(),
            createFunc: async travelGuide =>
            {
                if (!string.IsNullOrEmpty(travelGuide.ImageInBytes))
                {
                    travelGuide.Image = new FileUploadRequest()
                    {
                        Data = travelGuide.ImageInBytes, Extension = travelGuide.ImageExtension ?? string.Empty,
                        Name = $"{travelGuide.Title}_{Guid.NewGuid():N}"
                    };
                }

                if (travelGuide.TravelGuideGalleryImages?.Any() == true)
                {
                    foreach (var image in travelGuide.TravelGuideGalleryImages)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{travelGuide.Title}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }
                
                await TravelGuidesService.CreateAsync(travelGuide.Adapt<CreateTravelGuideRequest>());
                travelGuide.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, travelGuide) =>
            {
                if (!string.IsNullOrEmpty(travelGuide.ImageInBytes))
                {
                    travelGuide.DeleteCurrentImage = true;
                    travelGuide.Image = new FileUploadRequest()
                    {
                        Data = travelGuide.ImageInBytes, Extension = travelGuide.ImageExtension ?? string.Empty,
                        Name = $"{travelGuide.Title}_{Guid.NewGuid():N}"
                    };
                }

                if (travelGuide.TravelGuideGalleryImages?.Any() == true)
                {
                    foreach (var image in travelGuide.TravelGuideGalleryImages)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{travelGuide.Title}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }
                
                await TravelGuidesService.UpdateAsync(id, travelGuide.Adapt<UpdateTravelGuideRequest>());
                travelGuide.ImageInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await TravelGuidesService.DeleteAsync(id)
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
    
    private async Task UploadGalleryImage(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                var newImageRequest = new TravelGuideGalleryImageRequest()
                {
                    ImageExtension = fileUploadDetails.Extension,
                    ImageInBytes = fileUploadDetails.FileInBytes,
                    ImagePath = fileUploadDetails.FileInBytes,
                };
                
                (Context.AddEditModal.RequestModel.TravelGuideGalleryImages ??= new List<TravelGuideGalleryImageRequest>()).Insert(0, newImageRequest);
                SetGalleryImagesSortOrder();
            }
        }
    }
    
    private void SetGalleryImagesSortOrder(TravelGuideGalleryImageRequest? imageRequest = null, bool right = false)
    {
        if (Context.AddEditModal?.RequestModel == null ||
            Context.AddEditModal.RequestModel.TravelGuideGalleryImages?.Any() != true) return;
        if (imageRequest != null)
        {
            if (right)
            {
                var maxSortOrder = Context.AddEditModal.RequestModel.TravelGuideGalleryImages.Max(r => r.SortOrder);
                if (imageRequest.SortOrder < maxSortOrder)
                {
                    var nextRequest =
                        Context.AddEditModal.RequestModel.TravelGuideGalleryImages.FirstOrDefault(r =>
                            r.SortOrder == imageRequest.SortOrder + 1);
                    if (nextRequest != null)
                    {
                        nextRequest.SortOrder--;
                        imageRequest.SortOrder++;
                    }
                }
            }
            else
            {
                var previousRequest =
                    Context.AddEditModal.RequestModel.TravelGuideGalleryImages.FirstOrDefault(r =>
                        r.SortOrder == imageRequest.SortOrder - 1);
                if (previousRequest != null)
                {
                    previousRequest.SortOrder++;
                    imageRequest.SortOrder--;
                }
            }

            Context.AddEditModal.RequestModel.TravelGuideGalleryImages =
                Context.AddEditModal.RequestModel.TravelGuideGalleryImages.OrderBy(x => x.SortOrder).ToList();
        }
        else
        {
            for (var i = 0; i < Context.AddEditModal.RequestModel.TravelGuideGalleryImages.Count(); i++)
            {
                var image = Context.AddEditModal.RequestModel.TravelGuideGalleryImages.ToList()[i];
                image.SortOrder = i;
            }
        }

        Context.AddEditModal.ForceRender();
    }
    
    private void ClearGalleryImageInBytes(TravelGuideGalleryImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.TravelGuideGalleryImages?.Remove(image);
        Context.AddEditModal.ForceRender();
    }

    private void SetDeleteGalleryImageFlag(TravelGuideGalleryImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        image.ImagePath = string.Empty;
        image.DeleteCurrentImage = true;
        Context.AddEditModal.RequestModel.TravelGuideGalleryImages?.Remove(image);
        Context.AddEditModal.ForceRender();
    }
}

public class TravelGuideViewModel : UpdateTravelGuideRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}