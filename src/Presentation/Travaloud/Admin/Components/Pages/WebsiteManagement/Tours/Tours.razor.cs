using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Nextended.Core.Extensions;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;

public partial class Tours
{
    [Inject] protected IToursService ToursService { get; set; } = default!;

    [Inject] protected ITourCategoriesService TourCategoriesService { get; set; } = default!;

    [Inject] protected IDestinationsService DestinationsService { get; set; } = default!;

    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IPropertiesService PropertiesService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;
    
    // [Inject] protected IPartnersService SuppliersService { get; set; } = default!;
    
    private EntityServerTableContext<TourDto, DefaultIdType, TourViewModel> Context { get; set; } = default!;

    private EntityTable<TourDto, DefaultIdType, TourViewModel> _table = default!;

    [CascadingParameter] public MudCarousel<TourItinerarySectionRequest>? Carousel { get; set; }

    [CascadingParameter] private TravaloudTenantInfo? _tenantInfo { get; set; }

    private static Dictionary<string, bool> WizardSteps { get; set; } = new()
    {
        {"Basic Information", true},
        // {"Description", true},
        {"Pricing", true},
        {"Dates", true},
        {"Booking", true},
        {"Itineraries", true},
        {"SEO", true},
        {"Images", true}
    };

    private bool _canViewTourGroups;
    private bool _canViewTourCategories;

    protected override void OnAfterRender(bool firstRender)
    {
        if (_tenantInfo?.Id == "fuse")
        {
            WizardSteps["Itineraries"] = false;
            _canViewTourGroups = false;
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
        Context = new EntityServerTableContext<TourDto, DefaultIdType, TourViewModel>(
            entityName: L["Tour"],
            entityNamePlural: L["Tours"],
            entityResource: TravaloudResource.Tours,
            fields: [
                new EntityField<TourDto>(tour => tour.Name, L["Name"], "Name"),
                new EntityField<TourDto>(tour => tour.TourCategory?.Name, L["Category"], "TourCategory.Name")],
            enableAdvancedSearch: false,
            idFunc: tour => tour.Id,
            searchFunc: async filter => (await ToursService
                    .SearchAsync(filter.Adapt<SearchToursRequest>()))
                .Adapt<PaginationResponse<TourDto>>(),
            createFunc: async tour => await CreateTour(tour),
            getDetailsFunc: async (id) => await GetTour(id),
            getDefaultsFunc: async () => await GetCreateTourModel(),
            updateFunc: async (id, tour) => await UpdateTour(id, tour),
            exportAction: string.Empty,
            deleteFunc: async id => await ToursService.DeleteAsync(id)
        );
        return Task.CompletedTask;
    }

    private async Task<TourViewModel> GetTour(DefaultIdType id)
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

        var destinationsTask = Task.Run(() => DestinationsService.SearchAsync(new SearchDestinationsRequest()
        {
            PageNumber = 1,
            PageSize = int.MaxValue
        }));

        var propertiesTask = Task.Run(() => PropertiesService.SearchAsync(new SearchPropertiesRequest()
        {
            PageNumber = 1,
            PageSize = int.MaxValue
        }));

        var suppliersTask = Task.Run(() => UserService.SearchByDapperAsync(new SearchByDapperRequest
        {
            PageNumber = 1,
            PageSize = int.MaxValue,
            Role = TravaloudRoles.Supplier,
            TenantId = _tenantInfo?.Id!,
            OrderBy =
            [
                "FullName Ascending"
            ]
        }, CancellationToken.None));
        
        await Task.WhenAll(destinationsTask, propertiesTask, suppliersTask);

        parsedModel.Suppliers = suppliersTask.Result.Data;
        
        var destinations = destinationsTask.Result;

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

        var properties = propertiesTask.Result;

        if (properties?.Data == null) return parsedModel;
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
    }

    private async Task<TourViewModel> GetCreateTourModel()
    {
        var tour = new TourViewModel();

        var getTourCategories = Task.Run(() => TourCategoriesService.GetAllAsync(new GetTourCategoriesRequest()));
        // var getParentTourCategories = Task.Run(() => TourCategoriesService.GetAllAsync(new GetTourCategoriesRequest(true)));
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

        var suppliersTask = Task.Run(() => UserService.SearchByDapperAsync(new SearchByDapperRequest
        { 
            PageNumber = 1,
            PageSize = int.MaxValue,
            Role = TravaloudRoles.Supplier,
            TenantId = _tenantInfo?.Id!,
            OrderBy =
            [
                "FullName Ascending"
            ]
        }, CancellationToken.None));

        await Task.WhenAll(getTourCategories, getDestinations, getProperties, suppliersTask);

        var parsedTourCategories = getTourCategories.Result;
        
        var parentTourCategories = parsedTourCategories.Where(x => x.TopLevelCategory == true).OrderBy(x => x.Name).ToList();
        var tourCategories = parsedTourCategories.Where(x => !x.TopLevelCategory.HasValue || !x.TopLevelCategory.Value).OrderBy(x => x.Name).ToList();
        
        tour.TourCategories = tourCategories.Adapt<IList<TourCategoryRequest>>();
        tour.ParentTourCategories = parentTourCategories.Adapt<IList<TourCategoryRequest>>();
        tour.Suppliers = suppliersTask.Result.Data;
        
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
    }

    private async Task CreateTour(TourViewModel tour)
    {
        if (!string.IsNullOrEmpty(tour.ImageInBytes))
        {
            tour.Image = new FileUploadRequest()
            {
                Data = tour.ImageInBytes, Extension = tour.ImageExtension ?? string.Empty,
                Name = $"{tour.Name}_{DefaultIdType.NewGuid():N}"
            };
        }

        var parsedRequest = tour.Adapt<CreateTourRequest>();

        // if (!string.IsNullOrEmpty(parsedRequest.SelectedParentTourCategoriesString) &&
        //     parsedRequest.ParentTourCategories != null)
        // {
        //     parsedRequest.SelectedParentTourCategories = new List<Guid>();
        //
        //     foreach (var selectedCategory in parsedRequest.SelectedParentTourCategoriesString.Split(","))
        //     {
        //         var categoryParsed = selectedCategory.ToLower().Replace(" ", string.Empty);
        //         var matchedCategory = parsedRequest.ParentTourCategories.FirstOrDefault(x =>
        //             x.Name.ToLower().Replace(" ", string.Empty) == categoryParsed);
        //
        //         if (matchedCategory != null &&
        //             parsedRequest.SelectedParentTourCategories.All(x => x != matchedCategory.Id))
        //         {
        //             parsedRequest.SelectedParentTourCategories.Add(matchedCategory.Id);
        //         }
        //     }
        // }

        if (parsedRequest.Images?.Any() == true)
        {
            foreach (var image in parsedRequest.Images)
            {
                if (!string.IsNullOrEmpty(image.ImageInBytes))
                {
                    image.Image = new FileUploadRequest()
                    {
                        Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }
            }
        }

        tour.TourDestinationLookups = tour.SelectedDestinations;
        tour.TourPickupLocations = tour.SelectedPickupLocations;

        var newDates = tour.TourDates?.Where(x => x.IsCreate);
        if (newDates != null)
        {
            var tourDateRequests = newDates as TourDateRequest[] ?? newDates.ToArray();
        
            if (tourDateRequests.Length > 1000)
            {
                var filteredNewDates = tourDateRequests.Take(1000);
                tour.TourDates.RemoveRange(tourDateRequests);
                
                var itemsToAdd = filteredNewDates as TourDateRequest[] ?? filteredNewDates.ToArray();
                tour.TourDates.AddRange(itemsToAdd);
                
                Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {itemsToAdd.Last().StartDate?.ToShortDateString()} onwards have not been added.", Severity.Info);
            }
        }
        
        await ToursService.CreateAsync(parsedRequest);
        tour.ImageInBytes = string.Empty;
    }

    private async Task UpdateTour(DefaultIdType id, TourViewModel tour)
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
        
        if (tour.Images?.Any() == true)
        {
            foreach (var image in tour.Images)
            {
                if (!string.IsNullOrEmpty(image.ImageInBytes))
                {
                    image.Image = new FileUploadRequest()
                    {
                        Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty,
                        Name = $"{tour.Name}_{DefaultIdType.NewGuid():N}"
                    };
                }
            }
        }

        tour.TourDestinationLookups = tour.SelectedDestinations;
        tour.TourPickupLocations = tour.SelectedPickupLocations;

        var newDates = tour.TourDates?.Where(x => x.IsCreate);
        if (newDates != null)
        {
            var tourDateRequests = newDates as TourDateRequest[] ?? newDates.ToArray();
        
            if (tourDateRequests.Length > 1000)
            {
                var filteredNewDates = tourDateRequests.Take(1000);
                tour.TourDates.RemoveRange(tourDateRequests);
                
                var itemsToAdd = filteredNewDates as TourDateRequest[] ?? filteredNewDates.ToArray();
                tour.TourDates.AddRange(itemsToAdd);
                
                Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {itemsToAdd.Last().StartDate?.ToShortDateString()} onwards have not been added.", Severity.Info);
            }
        }

        await ToursService.UpdateAsync(id, tour.Adapt<UpdateTourRequest>());

        tour.ImageInBytes = string.Empty;
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
    public IList<UserDetailsDto>? Suppliers { get; set; } = new List<UserDetailsDto>();
}