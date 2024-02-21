using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;

public partial class Tours
{
    [Inject] protected IToursService ToursService { get; set; } = default!;

    private EntityServerTableContext<TourDto, Guid, TourViewModel> Context { get; set; } = default!;
    
    private EntityTable<TourDto, Guid, TourViewModel> _table = default!;

    [CascadingParameter] public MudCarousel<TourItinerarySectionRequest>? Carousel { get; set; }

    [CascadingParameter] private TravaloudTenantInfo? _tenantInfo { get; set; }
    
    private MudTable<TourItineraryRequest>? _itinerariesTable;

    private MudTable<TourPriceRequest>? _pricesTable;

    private MudTable<TourDateRequest>? _datesTable;

    public MudSelect<string>? TourGroupSelect;

    private string Tenant { get; set; } = default!;

    private static Dictionary<string, bool> WizardSteps { get; set; } = new()
    {
        {"Basic Information", true},
        {"Description", true},
        {"Pricing", true},
        {"Dates", true},
        {"Itineraries", true},
        {"SEO", true},
        {"Images", true}
    };

    private bool _canViewTourGroups;

    protected override void OnAfterRender(bool firstRender)
    {
        if (_tenantInfo?.Id == "fuse")
        {
            WizardSteps["Itineraries"] = false;
            _canViewTourGroups = false;
        }
        else
        {
            _canViewTourGroups = true;
        }

        
        base.OnAfterRender(firstRender);
    }

    protected override Task OnInitializedAsync()
    {
        Context = new EntityServerTableContext<TourDto, Guid, TourViewModel>(
            entityName: L["Tour"],
            entityNamePlural: L["Tours"],
            entityResource: TravaloudResource.Tours,
            fields: [new EntityField<TourDto>(tour => tour.Name, L["Name"], "Name")],
            enableAdvancedSearch: false,
            idFunc: tour => tour.Id,
            searchFunc: async filter => (await ToursService
                    .SearchAsync(filter.Adapt<SearchToursRequest>()))
                .Adapt<PaginationResponse<TourDto>>(),
            createFunc: async tour =>
            {
                if (!string.IsNullOrEmpty(tour.ImageInBytes))
                {
                    tour.Image = new FileUploadRequest()
                    {
                        Data = tour.ImageInBytes, Extension = tour.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{Guid.NewGuid():N}"
                    };
                }

                var parsedRequest = tour.Adapt<CreateTourRequest>();

                if (!string.IsNullOrEmpty(parsedRequest.SelectedParentTourCategoriesString) &&
                    parsedRequest.ParentTourCategories != null)
                {
                    parsedRequest.SelectedParentTourCategories = new List<Guid>();

                    foreach (var selectedCategory in parsedRequest.SelectedParentTourCategoriesString.Split(","))
                    {
                        var categoryParsed = selectedCategory.ToLower().Replace(" ", string.Empty);
                        var matchedCategory = parsedRequest.ParentTourCategories.FirstOrDefault(x =>
                            x.Name.ToLower().Replace(" ", string.Empty) == categoryParsed);

                        if (matchedCategory != null && parsedRequest.SelectedParentTourCategories.All(x => x != matchedCategory.Id))
                        {
                            parsedRequest.SelectedParentTourCategories.Add(matchedCategory.Id);
                        }
                    }
                }
                
                if (parsedRequest.Images?.Any() == true)
                {
                    foreach (var image in parsedRequest.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{tour.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }

                await ToursService.CreateAsync(parsedRequest);
                tour.ImageInBytes = string.Empty;
            },
            getDetailsFunc: async (id) =>
            {
                var tour = await ToursService.GetAsync(id);
                var parsedModel = tour.Adapt<TourViewModel>();
                if (tour != null)
                {
                    parsedModel.UseTourGroup = tour.TourCategoryId.HasValue;

                    if (!string.IsNullOrEmpty(tour.SelectedParentTourCategoriesString))
                    {
                        parsedModel.TourCategoriesOptions = tour.SelectedParentTourCategoriesString.Split(',')
                            .Select(x => x.TrimStart(' ')).ToHashSet<string>();
                    }
                }

                if (parsedModel is {TourPrices: not null, TourDates: not null})
                {
                    parsedModel.TourPrices = parsedModel.TourPrices.Select(x =>
                    {
                        x.Dates = parsedModel.TourDates.Where(td => td.TourPriceId == x.Id).ToList();
                        return x;
                    }).ToList();
                }

                if (parsedModel.Images?.Any() == true)
                {
                    parsedModel.Images = parsedModel.Images.OrderBy(x => x.SortOrder).ToList();
                }

                if (parsedModel.MaxCapacity is 99999)
                {
                    parsedModel.MaxCapacity = null;
                }

                return parsedModel;
            },
            getDefaultsFunc: async () =>
            {
                var tour = new TourViewModel();

                var tourCategories = await ToursService.GetCategoriesAsync();
                var parentTourCategories = await ToursService.GetParentCategoriesAsync();

                tour.TourCategories = tourCategories.Adapt<IList<TourCategoryRequest>>();
                tour.ParentTourCategories = parentTourCategories.Adapt<IList<TourCategoryRequest>>();

                return tour;
            },
            updateFunc: async (id, tour) =>
            {
                if (!string.IsNullOrEmpty(tour.ImageInBytes))
                {
                    tour.DeleteCurrentImage = true;
                    tour.Image = new FileUploadRequest()
                    {
                        Data = tour.ImageInBytes, Extension = tour.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{Guid.NewGuid():N}"
                    };
                }

                if (!string.IsNullOrEmpty(tour.SelectedParentTourCategoriesString) && tour.ParentTourCategories != null)
                {
                    tour.SelectedParentTourCategories = new List<Guid>();

                    foreach (var selectedCategory in tour.SelectedParentTourCategoriesString.Split(","))
                    {
                        var categoryParsed = selectedCategory.ToLower().Replace(" ", string.Empty);
                        var matchedCategory = tour.ParentTourCategories.FirstOrDefault(x =>
                            x.Name.ToLower().Replace(" ", string.Empty) == categoryParsed);

                        if (matchedCategory != null && tour.SelectedParentTourCategories.All(x => x != matchedCategory.Id))
                        {
                            tour.SelectedParentTourCategories.Add(matchedCategory.Id);
                        }
                    }
                }

                if (tour.Images?.Any() == true)
                {
                    foreach (var image in tour.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImageInBytes))
                        {
                            image.Image = new FileUploadRequest()
                            {
                                Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                                Name = $"{tour.Name}_{Guid.NewGuid():N}"
                            };
                        }
                    }
                }

                await ToursService.UpdateAsync(id, tour.Adapt<UpdateTourRequest>());

                tour.ImageInBytes = string.Empty;
            },
            exportAction: string.Empty,
            deleteFunc: async id => await ToursService.DeleteAsync(id)
        );
        return Task.CompletedTask;
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

    public async Task InvokePriceDialog(TourPriceRequest requestModel, TourViewModel tour, bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<TourPriceRequest>>(JsonSerializer.Serialize(tour.TourPrices)) ?? new List<TourPriceRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.WebsiteManagement.Tours.TourPrice.RequestModel), requestModel},
            {nameof(Dialogs.WebsiteManagement.Tours.TourPrice.Tour), tour},
            {nameof(Dialogs.WebsiteManagement.Tours.TourPrice.Context), Context},
            {nameof(Dialogs.WebsiteManagement.Tours.TourPrice.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<Dialogs.WebsiteManagement.Tours.TourPrice>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            tour.TourPrices = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemovePriceRow(TourViewModel tour, Guid id)
    {
        string deleteContent =
            L[
                "You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Price", id)}
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var tourPrice = tour.TourPrices?.FirstOrDefault(x => x.Id == id);
            if (tourPrice != null)
            {
                tour.TourPrices?.Remove(tourPrice);
            }

            Context.AddEditModal?.ForceRender();
        }
    }

    public async Task InvokeItineraryDialog(TourItineraryRequest requestModel, TourViewModel tour, bool isCreate = false)
    {
        if (isCreate)
        {
            (requestModel.Sections ??= new List<TourItinerarySectionRequest>()).Add(new TourItinerarySectionRequest());
        }

        var initialModel = JsonSerializer.Deserialize<IList<TourItineraryRequest>>(JsonSerializer.Serialize(tour.TourItineraries)) ?? new List<TourItineraryRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.WebsiteManagement.Tours.TourItinerary.RequestModel), requestModel},
            {nameof(Dialogs.WebsiteManagement.Tours.TourItinerary.Tour), tour},
            {nameof(Dialogs.WebsiteManagement.Tours.TourItinerary.Context), Context},
            {nameof(Dialogs.WebsiteManagement.Tours.TourItinerary.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<Dialogs.WebsiteManagement.Tours.TourItinerary>(string.Empty, parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            tour.TourItineraries = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveItineraryRow(TourViewModel tour, Guid id)
    {
        string deleteContent =
            L[
                "You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Itinerary", id)}
        };

        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var tourItinerary = tour.TourItineraries?.FirstOrDefault(x => x.Id == id);
            if (tourItinerary != null)
            {
                tour.TourItineraries?.Remove(tourItinerary);
            }

            Context.AddEditModal?.ForceRender();
        }
    }

    public async Task InvokeDateDialog(TourDateRequest requestModel, TourViewModel tour, bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<TourDateRequest>>(JsonSerializer.Serialize(tour.TourDates)) ?? new List<TourDateRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.WebsiteManagement.Tours.TourDate.RequestModel), requestModel},
            {nameof(Dialogs.WebsiteManagement.Tours.TourDate.Tour), tour},
            {nameof(Dialogs.WebsiteManagement.Tours.TourDate.Context), Context},
            {nameof(Dialogs.WebsiteManagement.Tours.TourDate.Id), isCreate ? null : requestModel.Id}
        };

        var dialog = await DialogService.ShowAsync<Dialogs.WebsiteManagement.Tours.TourDate>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            tour.TourDates = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task RemoveDateRow(TourViewModel tour, Guid id)
    {
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is not final. Refresh the page if you've made a mistake."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Date", id)}
        };

        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var tourDate = tour.TourDates?.FirstOrDefault(x => x.Id == id);
            if (tourDate != null)
            {
                tour.TourDates?.Remove(tourDate);
            }

            Context.AddEditModal?.ForceRender();
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

    private void ToggleTourGroup(TourViewModel context)
    {
        context.UseTourGroup = !context.UseTourGroup;

        if (context.UseTourGroup)
        {
            context.SelectedParentTourCategoriesString = string.Empty;
            context.SelectedParentTourCategories = new List<Guid>();
            context.TourCategoriesOptions = null;
        }
        else
        {
            context.TourCategoryId = null;
        }

        StateHasChanged();
        Context.AddEditModal?.ForceRender();
    }

    private async Task UploadSlideshowImage(InputFileChangeEventArgs e)
    {
        if (Context.AddEditModal != null)
        {
            var fileUploadDetails = await FileUploadHelper.UploadFile(e, Snackbar);

            if (fileUploadDetails != null)
            {
                var newImageRequest = new TourImageRequest()
                {
                    ImageExtension = fileUploadDetails.Extension,
                    ImageInBytes = fileUploadDetails.FileInBytes,
                    ImagePath = fileUploadDetails.FileInBytes,
                    ThumbnailImagePath = fileUploadDetails.FileInBytes
                };
                
                (Context.AddEditModal.RequestModel.Images ??= new List<TourImageRequest>()).Insert(0, newImageRequest);
                SetSlideshowImagesSortOrder();
            }
        }
    }

    private void SetSlideshowImagesSortOrder(TourImageRequest? imageRequest = null, bool right = false)
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
            for (var i = 0; i < Context.AddEditModal.RequestModel.Images.Count; i++)
            {
                var image = Context.AddEditModal.RequestModel.Images[i];
                image.SortOrder = i;
            }
        }

        Context.AddEditModal.ForceRender();
    }

    private void ClearSlideshowImageInBytes(TourImageRequest image)
    {
        if (Context.AddEditModal == null) return;
        
        image.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.Images?.Remove(image);
        Context.AddEditModal.ForceRender();
    }

    private void SetDeleteSlideshowImageFlag(TourImageRequest image)
    {
        if (Context.AddEditModal?.RequestModel == null) return;
        
        image.ImageInBytes = string.Empty;
        image.ImagePath = string.Empty;
        image.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }
}

public class TourViewModel : UpdateTourRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public bool UseTourGroup { get; set; }
    public IEnumerable<string>? TourCategoriesOptions { get; set; }
}