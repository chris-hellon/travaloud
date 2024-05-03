using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
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

    [Inject] protected IDestinationsService DestinationsService { get; set; } = default!;
    
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;
    
    [Inject] protected IPropertiesService PropertiesService { get; set; } = default!;

    private EntityServerTableContext<TourDto, Guid, TourViewModel> Context { get; set; } = default!;
    
    private EntityTable<TourDto, Guid, TourViewModel> _table = default!;

    [CascadingParameter] public MudCarousel<TourItinerarySectionRequest>? Carousel { get; set; }

    [CascadingParameter] private TravaloudTenantInfo? _tenantInfo { get; set; }

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
    private bool _canViewDestinations;
    private bool _canViewTourCategories;

    protected override void OnAfterRender(bool firstRender)
    {
        if (_tenantInfo?.Id == "fuse")
        {
            WizardSteps["Itineraries"] = false;
            _canViewTourGroups = false;
            _canViewDestinations = true;
            _canViewTourCategories = true;
        }
        else
        {
            _canViewTourGroups = true;
            _canViewTourCategories = false;
            WizardSteps["Itineraries"] = true;
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
            // editNavigateTo: dto => $"/management/tours/{dto.Id}",
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
                
                tour.TourDestinationLookups = tour.SelectedDestinations;
                tour.TourPickupLocations = tour.SelectedPickupLocations;

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
            
                var destinations = await DestinationsService.SearchAsync(new SearchDestinationsRequest()
                {
                    PageNumber = 1,
                    PageSize = 99999
                });
            
                if (destinations?.Data != null)
                {
                    foreach (var destination in destinations.Data)
                    {
                        parsedModel.Destinations?.Add(
                            new TourDestinationLookupRequest(tour.Id, destination.Id, destination.Name));
                    }
            
                    if (parsedModel.TourDestinationLookups == null) return parsedModel;
                    {
                        var selectedDestinations = parsedModel.TourDestinationLookups.Select(destinationLookup =>
                                parsedModel.Destinations.FirstOrDefault(x =>
                                    x.DestinationId == destinationLookup.DestinationId))
                            .Select(destination => new TourDestinationLookupRequest(
                                tour.Id,
                                destination.DestinationId,
                                destinations.Data.First(x => x.Id == destination.DestinationId).Name))
                            .ToList();
            
                        parsedModel.SelectedDestinations = selectedDestinations;
                    }
                }
                
                var properties = await PropertiesService.SearchAsync(new SearchPropertiesRequest()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });
                
                if (properties?.Data != null)
                {
                    foreach (var property in properties.Data)
                    {
                        parsedModel.PickupLocations?.Add(
                            new TourPickupLocationRequest(tour.Id, property.Id, property.Name));
                    }
            
                    if (parsedModel.TourPickupLocations == null) return parsedModel;
                    {
                        var selectedDestinations = parsedModel.TourPickupLocations.Select(pickupLocation =>
                                parsedModel.PickupLocations.FirstOrDefault(x =>
                                    x.PropertyId == pickupLocation.PropertyId))
                            .Select(pickupLocation => new TourPickupLocationRequest(
                                pickupLocation.Id,
                                tour.Id,
                                pickupLocation.PropertyId,
                                properties.Data.First(x => x.Id == pickupLocation.PropertyId).Name))
                            .ToList();
            
                        parsedModel.SelectedPickupLocations = selectedDestinations;
                    }
                }
            
                return parsedModel;
            },
            getDefaultsFunc: async () =>
            {
                var tour = new TourViewModel();
                
                var getTourCategories = Task.Run(() => ToursService.GetCategoriesAsync());
                var getParentTourCategories = Task.Run(() => ToursService.GetParentCategoriesAsync());
                var getDestinations = Task.Run(() => DestinationsService.SearchAsync(new SearchDestinationsRequest()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                }));

                var getProperties = Task.Run(() => PropertiesService.SearchAsync(new SearchPropertiesRequest()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                }));
                
                await Task.WhenAll(getTourCategories, getParentTourCategories, getDestinations, getProperties);
                
                tour.TourCategories = getTourCategories.Result.Adapt<IList<TourCategoryRequest>>();
                tour.ParentTourCategories = getParentTourCategories.Result.Adapt<IList<TourCategoryRequest>>();

                var destinations = getDestinations.Result.Data;

                foreach (var destination in destinations)
                {
                    tour.Destinations.Add(new TourDestinationLookupRequest(tour.Id, destination.Id, destination.Name));
                }
                
                var properties = getProperties.Result.Data;

                foreach (var property in properties)
                {
                    tour.PickupLocations.Add(new TourPickupLocationRequest(tour.Id, property.Id, property.Name));
                }

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

                tour.TourDestinationLookups = tour.SelectedDestinations;
                tour.TourPickupLocations = tour.SelectedPickupLocations;

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
}

public class TourViewModel : UpdateTourRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
    public bool UseTourGroup { get; set; }
    public IEnumerable<string>? TourCategoriesOptions { get; set; }
    public IList<TourDestinationLookupRequest>? Destinations { get; set; } = new List<TourDestinationLookupRequest>();
    public IList<TourPickupLocationRequest>? PickupLocations { get; set; } = new List<TourPickupLocationRequest>();
    public IEnumerable<TourDestinationLookupRequest>? SelectedDestinations = [];
    public IEnumerable<TourPickupLocationRequest>? SelectedPickupLocations = [];
}