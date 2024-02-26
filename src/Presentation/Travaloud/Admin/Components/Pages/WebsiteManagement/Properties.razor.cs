using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.Json;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Shared.Authorization;
using Travaloud.Admin.Components.Dialogs.WebsiteManagement.Properties;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement;

public partial class Properties 
{
    [Inject] private IPropertiesService PropertiesService { get; set; } = default!;

    [Inject] private IDestinationsService DestinationsService { get; set; } = default!;

    private EntityServerTableContext<PropertyDto, Guid, PropertyViewModel> Context { get; set; } = default!;

    private EditContext? EditContext { get; set; }

    private EntityTable<PropertyDto, Guid, PropertyViewModel> _table = default!;

    private ICollection<DestinationDto>? Destinations { get; set; }

    private MudTable<PropertyDirectionRequest>? _directionsTable;

    private MudTable<PropertyRoomRequest>? _roomsTable;

    private static Dictionary<string, bool> WizardSteps => new()
    {
        {"Basic Information", true},
        {"Description", true},
        {"Directions", true},
        {"Rooms", true},
        {"SEO", true},
        {"Images", true},
    };

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<PropertyDto, Guid, PropertyViewModel>(
            entityName: L["Property"],
            entityNamePlural: L["Properties"],
            entityResource: TravaloudResource.Properties,
            fields: [new EntityField<PropertyDto>(dest => dest.Name, L["Name"], "Name")],
            enableAdvancedSearch: false,
            idFunc: property => property.Id,
            searchFunc: async filter => (await PropertiesService.SearchAsync(filter.Adapt<SearchPropertiesRequest>()))
                .Adapt<PaginationResponse<PropertyDto>>(),
            getDefaultsFunc: async () =>
            {
                var destinations =
                    await DestinationsService.SearchAsync(
                        new SearchDestinationsRequest() {PageNumber = 1, PageSize = 99999});
                Destinations = destinations?.Data;

                return new PropertyViewModel();
            },
            getDetailsFunc: async (id) =>
            {
                var destinations =
                    await DestinationsService.SearchAsync(
                        new SearchDestinationsRequest() {PageNumber = 1, PageSize = 99999});
                Destinations = destinations?.Data;

                var property = await PropertiesService.GetAsync(id);
                var adaptedProperty = property.Adapt<PropertyViewModel>();

                if (property?.PropertyDestinationLookups?.Any() == true)
                {
                    adaptedProperty.DestinationId = property.PropertyDestinationLookups.First().DestinationId;
                }

                if (adaptedProperty.Images?.Any() == true)
                {
                    adaptedProperty.Images = adaptedProperty.Images.OrderBy(x => x.SortOrder).ToList();
                }

                return adaptedProperty;
            },
            createFunc: async property =>
            {
                if (!string.IsNullOrEmpty(property.ImageInBytes))
                {
                    property.Image = new FileUploadRequest()
                    {
                        Data = property.ImageInBytes, Extension = property.ImageExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (!string.IsNullOrEmpty(property.VideoInBytes))
                {
                    property.Video = new FileUploadRequest()
                    {
                        Data = property.VideoInBytes, Extension = property.VideoExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (!string.IsNullOrEmpty(property.MobileVideoInBytes))
                {
                    property.MobileVideo = new FileUploadRequest()
                    {
                        Data = property.MobileVideoInBytes, Extension = property.MobileVideoExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (property.Rooms?.Any() == true)
                {
                    foreach (var parsedRoom in property.Rooms)
                    {
                        if (!string.IsNullOrEmpty(parsedRoom.ImageInBytes))
                        {
                            parsedRoom.Image = new FileUploadRequest()
                            {
                                Data = parsedRoom.ImageInBytes, Extension = parsedRoom.ImageExtension ?? string.Empty,
                                Name = $"{property.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }

                if (property.DestinationId.HasValue)
                {
                    property.PropertyDestinationLookups = new List<PropertyDestinationLookupRequest>()
                    {
                        new() {DestinationId = property.DestinationId.Value}
                    };
                }

                if (property.Images?.Any() == true)
                {
                    foreach (var image in property.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{property.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }
                
                await PropertiesService.CreateAsync(property.Adapt<CreatePropertyRequest>());
                property.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, property) =>
            {
                if (!string.IsNullOrEmpty(property.ImageInBytes))
                {
                    property.DeleteCurrentImage = true;
                    property.Image = new FileUploadRequest()
                    {
                        Data = property.ImageInBytes, Extension = property.ImageExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (!string.IsNullOrEmpty(property.VideoInBytes))
                {
                    property.DeleteCurrentVideo = true;
                    property.Video = new FileUploadRequest()
                    {
                        Data = property.VideoInBytes, Extension = property.VideoExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (!string.IsNullOrEmpty(property.MobileVideoInBytes))
                {
                    property.DeleteCurrentMobileVideo = true;
                    property.MobileVideo = new FileUploadRequest()
                    {
                        Data = property.MobileVideoInBytes, Extension = property.MobileVideoExtension ?? string.Empty,
                        Name = $"{property.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (property.Rooms?.Any() == true)
                {
                    foreach (var parsedRoom in property.Rooms)
                    {
                        if (!string.IsNullOrEmpty(parsedRoom.ImageInBytes))
                        {
                            parsedRoom.Image = new FileUploadRequest()
                            {
                                Data = parsedRoom.ImageInBytes, Extension = parsedRoom.ImageExtension ?? string.Empty,
                                Name = $"{property.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }

                if (property.DestinationId.HasValue)
                {
                    property.PropertyDestinationLookups = new List<PropertyDestinationLookupRequest>()
                    {
                        new() {DestinationId = property.DestinationId.Value}
                    };
                }

                if (property.Images?.Any() == true)
                {
                    foreach (var image in property.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{property.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }

                await PropertiesService.UpdateAsync(id, property.Adapt<UpdatePropertyRequest>());
                property.ImageInBytes = string.Empty;
                property.VideoInBytes = string.Empty;
                property.MobileVideoInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await PropertiesService.DeleteAsync(id)
        );
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

    private async Task UploadVideo(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar, false);

            if (fileUploadDetails != null)
            {
                Context.AddEditModal.RequestModel.VideoExtension = fileUploadDetails.Extension;
                Context.AddEditModal.RequestModel.VideoInBytes = fileUploadDetails.FileInBytes;
                Context.AddEditModal.ForceRender();
            }
        }
    }

    private async Task UploadSlideshowImage(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                var newImageRequest = new PropertyImageRequest()
                {
                    ImageExtension = fileUploadDetails.Extension,
                    ImageInBytes = fileUploadDetails.FileInBytes,
                    ImagePath = fileUploadDetails.FileInBytes,
                    ThumbnailImagePath = fileUploadDetails.FileInBytes
                };
                
                (Context.AddEditModal.RequestModel.Images ??= new List<PropertyImageRequest>()).Insert(0, newImageRequest);
                SetSlideshowImagesSortOrder();
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

    public void ClearVideoInBytes()
    {
        if (Context.AddEditModal == null) return;

        Context.AddEditModal.RequestModel.VideoInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentVideoFlag()
    {
        if (Context.AddEditModal == null) return;

        Context.AddEditModal.RequestModel.VideoInBytes = string.Empty;
        Context.AddEditModal.RequestModel.VideoPath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentVideo = true;
        Context.AddEditModal.ForceRender();
    }

    private void ClearSlideshowImageInBytes(PropertyImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.Images?.Remove(image);
        Context.AddEditModal.ForceRender();
    }

    private void SetDeleteSlideshowImageFlag(PropertyImageRequest image)
    {
        if (Context.AddEditModal == null) return;

        image.ImageInBytes = string.Empty;
        image.ImagePath = string.Empty;
        image.DeleteCurrentImage = true;
        Context.AddEditModal.RequestModel.Images?.Remove(image);
        Context.AddEditModal.ForceRender();
    }

    public async Task InvokeDirectionsDialog(PropertyDirectionRequest requestModel, PropertyViewModel property, bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<PropertyDirectionRequest>>(JsonSerializer.Serialize(property.Directions)) ?? new List<PropertyDirectionRequest>();
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(PropertyDirections.RequestModel), requestModel},
            {nameof(PropertyDirections.Property), property},
            {nameof(PropertyDirections.Context), Context},
            {nameof(PropertyDirections.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<PropertyDirections>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            property.Directions = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveDirectionRow(PropertyViewModel property, Guid id)
    {
        string deleteContent =
            L[
                "You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Direction", id)}
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var direction = property.Directions?.FirstOrDefault(x => x.Id == id);
            if (direction != null)
            {
                property.Directions?.Remove(direction);
            }

            Context.AddEditModal?.ForceRender();
        }
    }

    public async Task InvokeRoomDialog(PropertyRoomRequest requestModel, PropertyViewModel property, bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<PropertyRoomRequest>>(JsonSerializer.Serialize(property.Rooms)) ?? new List<PropertyRoomRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(PropertyRoom.RequestModel), requestModel},
            {nameof(PropertyRoom.Property), property},
            {nameof(PropertyRoom.Context), Context},
            {nameof(PropertyRoom.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<PropertyRoom>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            property.Rooms = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveRoomRow(PropertyViewModel property, Guid id)
    {
        string deleteContent =
            L[
                "You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Room", id)}
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var room = property.Rooms?.FirstOrDefault(x => x.Id == id);
            if (room != null)
            {
                property.Rooms?.Remove(room);
            }

            Context.AddEditModal?.ForceRender();
        }
    }

    private void SetSlideshowImagesSortOrder(PropertyImageRequest? imageRequest = null, bool right = false)
    {
        if (Context.AddEditModal?.RequestModel == null ||
            Context.AddEditModal.RequestModel.Images?.Any() != true) return;
        if (imageRequest != null)
        {
            if (right)
            {
                var maxSortOrder = Context.AddEditModal.RequestModel.Images.Max(r => r.SortOrder);
                if (imageRequest.SortOrder < maxSortOrder)
                {
                    var nextRequest =
                        Context.AddEditModal.RequestModel.Images.FirstOrDefault(r =>
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
                    Context.AddEditModal.RequestModel.Images.FirstOrDefault(r =>
                        r.SortOrder == imageRequest.SortOrder - 1);
                if (previousRequest != null)
                {
                    previousRequest.SortOrder++;
                    imageRequest.SortOrder--;
                }
            }

            Context.AddEditModal.RequestModel.Images =
                Context.AddEditModal.RequestModel.Images.OrderBy(x => x.SortOrder).ToList();
        }
        else
        {
            for (var i = 0; i < Context.AddEditModal.RequestModel.Images.Count(); i++)
            {
                var image = Context.AddEditModal.RequestModel.Images.ToList()[i];
                image.SortOrder = i;
            }
        }

        Context.AddEditModal.ForceRender();
    }
}

public class PropertyViewModel : UpdatePropertyRequest
{
    public string? ImagePath { get; set; }
    public string? VideoPath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public string? VideoInBytes { get; set; }
    public string? VideoExtension { get; set; }
    public string? MobileVideoInBytes { get; set; }
    public string? MobileVideoExtension { get; set; }
    public Guid? DestinationId { get; set; }
}