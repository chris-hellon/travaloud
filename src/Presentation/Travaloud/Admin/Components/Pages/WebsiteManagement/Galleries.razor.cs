using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Galleries.Commands;
using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class Galleries
{
    [Inject] protected IGalleriesService GalleriesService { get; set; } = default!;

    private EntityServerTableContext<GalleryDto, Guid, UpdateGalleryRequest> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<GalleryDto, Guid, UpdateGalleryRequest> _table = default!;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        {"Basic Information", true},
        {"SEO", true},
        {"Images", true},
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<GalleryDto, Guid, UpdateGalleryRequest>(
            entityName: L["Gallery"],
            entityNamePlural: L["Galleries"],
            entityResource: TravaloudResource.Gallery,
            fields: [
                new EntityField<GalleryDto>(gallery => gallery.Title, L["Name"], "Name"),
                new EntityField<GalleryDto>(gallery => gallery.CreatedOn, L["Created On"], "CreatedOn")
            ],
            enableAdvancedSearch: false,
            idFunc: destination => destination.Id,
            searchFunc: async filter => (await GalleriesService
                    .SearchAsync(filter.Adapt<SearchGalleriesRequest>()))
                .Adapt<PaginationResponse<GalleryDto>>(),
            getDetailsFunc: async (id) =>
            {
                var gallery = await GalleriesService.GetAsync(id);
                var adaptedGallery = gallery.Adapt<UpdateGalleryRequest>();
                
                if (adaptedGallery.GalleryImages?.Any() == true)
                {
                    adaptedGallery.GalleryImages = adaptedGallery.GalleryImages.OrderBy(x => x.SortOrder).ToList();
                }

                return adaptedGallery;
            },
            createFunc: async gallery =>
            {
                if (gallery.GalleryImages?.Any() == true)
                {
                    foreach (var image in gallery.GalleryImages)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{gallery.Title}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }
                
                await GalleriesService.CreateAsync(gallery.Adapt<CreateGalleryRequest>());
            },
            updateFunc: async (id, gallery) =>
            {
                if (gallery.GalleryImages?.Any() == true)
                {
                    foreach (var image in gallery.GalleryImages)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{gallery.Title}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }
                
                await GalleriesService.UpdateAsync(id, gallery.Adapt<UpdateGalleryRequest>());
            },
            exportAction: string.Empty,
            deleteFunc: async id => await GalleriesService.DeleteAsync(id)
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
    private async Task UploadGalleryImage(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                var newImageRequest = new GalleryImageRequest()
                {
                    ImageExtension = fileUploadDetails.Extension,
                    ImageInBytes = fileUploadDetails.FileInBytes,
                    ImagePath = fileUploadDetails.FileInBytes,
                };
                
                (Context.AddEditModal.RequestModel.GalleryImages ??= new List<GalleryImageRequest>()).Insert(0, newImageRequest);
                SetGalleryImagesSortOrder();
            }
        }
    }
    
    private void SetGalleryImagesSortOrder(GalleryImageRequest? imageRequest = null, bool right = false)
    {
        if (Context.AddEditModal?.RequestModel == null ||
            Context.AddEditModal.RequestModel.GalleryImages?.Any() != true) return;
        if (imageRequest != null)
        {
            if (right)
            {
                var maxSortOrder = Context.AddEditModal.RequestModel.GalleryImages.Max(r => r.SortOrder);
                if (imageRequest.SortOrder < maxSortOrder)
                {
                    var nextRequest =
                        Context.AddEditModal.RequestModel.GalleryImages.FirstOrDefault(r =>
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
                    Context.AddEditModal.RequestModel.GalleryImages.FirstOrDefault(r =>
                        r.SortOrder == imageRequest.SortOrder - 1);
                if (previousRequest != null)
                {
                    previousRequest.SortOrder++;
                    imageRequest.SortOrder--;
                }
            }

            Context.AddEditModal.RequestModel.GalleryImages =
                Context.AddEditModal.RequestModel.GalleryImages.OrderBy(x => x.SortOrder).ToList();
        }
        else
        {
            for (var i = 0; i < Context.AddEditModal.RequestModel.GalleryImages.Count; i++)
            {
                var image = Context.AddEditModal.RequestModel.GalleryImages[i];
                image.SortOrder = i;
            }
        }

        Context.AddEditModal.ForceRender();
    }
    
    private void ClearGalleryImageInBytes(GalleryImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.GalleryImages?.Remove(image);
        Context.AddEditModal.ForceRender();
    }

    private void SetDeleteGalleryImageFlag(GalleryImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        image.ImagePath = string.Empty;
        image.DeleteCurrentImage = true;
        Context.AddEditModal.RequestModel.GalleryImages?.Remove(image);
        Context.AddEditModal.ForceRender();
    }
}