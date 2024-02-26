using System.Collections.ObjectModel;

namespace Travaloud.Shared.Authorization;

public static class TravaloudAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class TravaloudResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Brands = nameof(Brands);
    public const string Destinations = nameof(Destinations);
    public const string Tours = nameof(Tours);
    public const string Guests = nameof(Guests);
    public const string Partners = nameof(Partners);
    public const string Properties = nameof(Properties);
    public const string Bookings = nameof(Bookings);
    public const string Services = nameof(Services);
    public const string Events = nameof(Events);
    public const string JobVacancies = nameof(JobVacancies);
    public const string Enquiries = nameof(Enquiries);
    public const string Pages = nameof(Pages);
    public const string TravelGuides = nameof(TravelGuides);
    public const string Gallery = nameof(Gallery);
    public const string Settings = nameof(Settings);
}

public static class TravaloudPermissions
{
    private static readonly TravaloudPermission[] _all = new TravaloudPermission[]
        {
            new("View Dashboard", TravaloudAction.View, TravaloudResource.Dashboard),
            new("View Hangfire", TravaloudAction.View, TravaloudResource.Hangfire),
            new("View Users", TravaloudAction.View, TravaloudResource.Users),
            new("Search Users", TravaloudAction.Search, TravaloudResource.Users),
            new("Create Users", TravaloudAction.Create, TravaloudResource.Users),
            new("Update Users", TravaloudAction.Update, TravaloudResource.Users),
            new("Delete Users", TravaloudAction.Delete, TravaloudResource.Users),
            new("Export Users", TravaloudAction.Export, TravaloudResource.Users),
            new("View UserRoles", TravaloudAction.View, TravaloudResource.UserRoles),
            new("Update UserRoles", TravaloudAction.Update, TravaloudResource.UserRoles),
            new("View Roles", TravaloudAction.View, TravaloudResource.Roles),
            new("Create Roles", TravaloudAction.Create, TravaloudResource.Roles),
            new("Update Roles", TravaloudAction.Update, TravaloudResource.Roles),
            new("Delete Roles", TravaloudAction.Delete, TravaloudResource.Roles),
            new("View RoleClaims", TravaloudAction.View, TravaloudResource.RoleClaims),
            new("Update RoleClaims", TravaloudAction.Update, TravaloudResource.RoleClaims),
            new("View Products", TravaloudAction.View, TravaloudResource.Products, IsBasic: true),
            new("Search Products", TravaloudAction.Search, TravaloudResource.Products, IsBasic: true),
            new("Create Products", TravaloudAction.Create, TravaloudResource.Products),
            new("Update Products", TravaloudAction.Update, TravaloudResource.Products),
            new("Delete Products", TravaloudAction.Delete, TravaloudResource.Products),
            new("Export Products", TravaloudAction.Export, TravaloudResource.Products),
            new("View Brands", TravaloudAction.View, TravaloudResource.Brands, IsBasic: true),
            new("Search Brands", TravaloudAction.Search, TravaloudResource.Brands, IsBasic: true),
            new("Create Brands", TravaloudAction.Create, TravaloudResource.Brands),
            new("Update Brands", TravaloudAction.Update, TravaloudResource.Brands),
            new("Delete Brands", TravaloudAction.Delete, TravaloudResource.Brands),
            new("Generate Brands", TravaloudAction.Generate, TravaloudResource.Brands),
            new("Clean Brands", TravaloudAction.Clean, TravaloudResource.Brands),
            new("View Destinations", TravaloudAction.View, TravaloudResource.Destinations, IsBasic: true, IsPropertyManager: true),
            new("Search Destinations", TravaloudAction.Search, TravaloudResource.Destinations, IsBasic: true, IsPropertyManager: true),
            new("Create Destinations", TravaloudAction.Create, TravaloudResource.Destinations, IsPropertyManager: true),
            new("Update Destinations", TravaloudAction.Update, TravaloudResource.Destinations, IsPropertyManager: true),
            new("Delete Destinations", TravaloudAction.Delete, TravaloudResource.Destinations, IsPropertyManager: true),
            new("Export Destinations", TravaloudAction.Export, TravaloudResource.Destinations, IsPropertyManager: true),
            new("View Tours", TravaloudAction.View, TravaloudResource.Tours, IsBasic: true, IsTourManager: true),
            new("Search Tours", TravaloudAction.Search, TravaloudResource.Tours, IsBasic: true, IsTourManager: true),
            new("Create Tours", TravaloudAction.Create, TravaloudResource.Tours, IsTourManager: true),
            new("Update Tours", TravaloudAction.Update, TravaloudResource.Tours, IsTourManager: true),
            new("Delete Tours", TravaloudAction.Delete, TravaloudResource.Tours, IsTourManager: true),
            new("Export Tours", TravaloudAction.Export, TravaloudResource.Tours, IsTourManager: true),
            new("View Guests", TravaloudAction.View, TravaloudResource.Guests, IsBasic: true, IsTourManager: true, IsPropertyManager: true),
            new("Search Guests", TravaloudAction.Search, TravaloudResource.Guests, IsBasic: true, IsTourManager: true, IsPropertyManager: true),
            new("Create Guests", TravaloudAction.Create, TravaloudResource.Guests, IsTourManager: true, IsPropertyManager: true),
            new("Update Guests", TravaloudAction.Update, TravaloudResource.Guests, IsTourManager: true, IsPropertyManager: true),
            new("Delete Guests", TravaloudAction.Delete, TravaloudResource.Guests, IsTourManager: true, IsPropertyManager: true),
            new("Export Guests", TravaloudAction.Export, TravaloudResource.Guests, IsTourManager: true, IsPropertyManager: true),
            new("View Partners", TravaloudAction.View, TravaloudResource.Partners, IsBasic: true),
            new("Search Partners", TravaloudAction.Search, TravaloudResource.Partners, IsBasic: true),
            new("Create Partners", TravaloudAction.Create, TravaloudResource.Partners),
            new("Update Partners", TravaloudAction.Update, TravaloudResource.Partners),
            new("Delete Partners", TravaloudAction.Delete, TravaloudResource.Partners),
            new("Export Partners", TravaloudAction.Export, TravaloudResource.Partners),
            new("View Properties", TravaloudAction.View, TravaloudResource.Properties, IsBasic: true, IsPropertyManager: true),
            new("Search Properties", TravaloudAction.Search, TravaloudResource.Properties, IsBasic: true, IsPropertyManager: true),
            new("Create Properties", TravaloudAction.Create, TravaloudResource.Properties, IsPropertyManager: true),
            new("Update Properties", TravaloudAction.Update, TravaloudResource.Properties, IsPropertyManager: true),
            new("Delete Properties", TravaloudAction.Delete, TravaloudResource.Properties, IsPropertyManager: true),
            new("Export Properties", TravaloudAction.Export, TravaloudResource.Properties, IsPropertyManager: true),
            new("View Bookings", TravaloudAction.View, TravaloudResource.Bookings, IsBasic: true, IsTourManager: true, IsPropertyManager: true),
            new("Search Bookings", TravaloudAction.Search, TravaloudResource.Bookings, IsBasic: true, IsTourManager: true, IsPropertyManager: true),
            new("Create Bookings", TravaloudAction.Create, TravaloudResource.Bookings, IsTourManager: true, IsPropertyManager: true),
            new("Update Bookings", TravaloudAction.Update, TravaloudResource.Bookings, IsTourManager: true, IsPropertyManager: true),
            new("Delete Bookings", TravaloudAction.Delete, TravaloudResource.Bookings, IsTourManager: true, IsPropertyManager: true),
            new("Export Bookings", TravaloudAction.Export, TravaloudResource.Bookings, IsTourManager: true, IsPropertyManager: true),
            new("View Services", TravaloudAction.View, TravaloudResource.Services, IsBasic: true),
            new("Search Services", TravaloudAction.Search, TravaloudResource.Services, IsBasic: true),
            new("Create Services", TravaloudAction.Create, TravaloudResource.Services),
            new("Update Services", TravaloudAction.Update, TravaloudResource.Services),
            new("Delete Services", TravaloudAction.Delete, TravaloudResource.Services),
            new("Export Services", TravaloudAction.Export, TravaloudResource.Services),
            new("View Events", TravaloudAction.View, TravaloudResource.Events, IsBasic: true),
            new("Search Events", TravaloudAction.Search, TravaloudResource.Events, IsBasic: true),
            new("Create Events", TravaloudAction.Create, TravaloudResource.Events),
            new("Update Events", TravaloudAction.Update, TravaloudResource.Events),
            new("Delete Events", TravaloudAction.Delete, TravaloudResource.Events),
            new("Export Events", TravaloudAction.Export, TravaloudResource.Events),
            new("View Job Vacancies", TravaloudAction.View, TravaloudResource.JobVacancies, IsBasic: true),
            new("Search Job Vacancies", TravaloudAction.Search, TravaloudResource.JobVacancies, IsBasic: true),
            new("Create Job Vacancies", TravaloudAction.Create, TravaloudResource.JobVacancies),
            new("Update Job Vacancies", TravaloudAction.Update, TravaloudResource.JobVacancies),
            new("Delete Job Vacancies", TravaloudAction.Delete, TravaloudResource.JobVacancies),
            new("Export Job Vacancies", TravaloudAction.Export, TravaloudResource.JobVacancies),
            new("View Enquiries", TravaloudAction.View, TravaloudResource.Enquiries, IsBasic: true),
            new("Search Enquiries", TravaloudAction.Search, TravaloudResource.Enquiries, IsBasic: true),
            new("Create Enquiries", TravaloudAction.Create, TravaloudResource.Enquiries),
            new("Update Enquiries", TravaloudAction.Update, TravaloudResource.Enquiries),
            new("Delete Enquiries", TravaloudAction.Delete, TravaloudResource.Enquiries),
            new("Export Enquiries", TravaloudAction.Export, TravaloudResource.Enquiries),
            new("View Pages", TravaloudAction.View, TravaloudResource.Pages, IsBasic: true),
            new("Search Pages", TravaloudAction.Search, TravaloudResource.Pages, IsBasic: true),
            new("Create Pages", TravaloudAction.Create, TravaloudResource.Pages),
            new("Update Pages", TravaloudAction.Update, TravaloudResource.Pages),
            new("Delete Pages", TravaloudAction.Delete, TravaloudResource.Pages),
            new("Export Pages", TravaloudAction.Export, TravaloudResource.Pages),
            new("View Travel Guides", TravaloudAction.View, TravaloudResource.TravelGuides, IsBasic: true),
            new("Search Travel Guides", TravaloudAction.Search, TravaloudResource.TravelGuides, IsBasic: true),
            new("Create Travel Guides", TravaloudAction.Create, TravaloudResource.TravelGuides),
            new("Update Travel Guides", TravaloudAction.Update, TravaloudResource.TravelGuides),
            new("Delete Travel Guides", TravaloudAction.Delete, TravaloudResource.TravelGuides),
            new("Export Travel Guides", TravaloudAction.Export, TravaloudResource.TravelGuides),
            new("View Gallery", TravaloudAction.View, TravaloudResource.Gallery, IsBasic: true),
            new("Search Gallery", TravaloudAction.Search, TravaloudResource.Gallery, IsBasic: true),
            new("Create Gallery", TravaloudAction.Create, TravaloudResource.Gallery),
            new("Update Gallery", TravaloudAction.Update, TravaloudResource.Gallery),
            new("Delete Gallery", TravaloudAction.Delete, TravaloudResource.Gallery),
            new("Export Gallery", TravaloudAction.Export, TravaloudResource.Gallery),
            new("View Settings", TravaloudAction.View, TravaloudResource.Settings),
            new("Update Settings", TravaloudAction.Update, TravaloudResource.Settings),
            new("View Tenants", TravaloudAction.View, TravaloudResource.Tenants, IsRoot: true),
            new("Create Tenants", TravaloudAction.Create, TravaloudResource.Tenants, IsRoot: true),
            new("Update Tenants", TravaloudAction.Update, TravaloudResource.Tenants, IsRoot: true),
            new("Upgrade Tenant Subscription", TravaloudAction.UpgradeSubscription, TravaloudResource.Tenants, IsRoot: true)
        };

    public static IReadOnlyList<TravaloudPermission> All { get; } = new ReadOnlyCollection<TravaloudPermission>(_all);
    public static IReadOnlyList<TravaloudPermission> Root { get; } = new ReadOnlyCollection<TravaloudPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<TravaloudPermission> Admin { get; } = new ReadOnlyCollection<TravaloudPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<TravaloudPermission> Basic { get; } = new ReadOnlyCollection<TravaloudPermission>(_all.Where(p => p.IsBasic).ToArray());
    public static IReadOnlyList<TravaloudPermission> TourManager { get; } = new ReadOnlyCollection<TravaloudPermission>(_all.Where(p => p.IsTourManager).ToArray());
    public static IReadOnlyList<TravaloudPermission> PropertyManager { get; } = new ReadOnlyCollection<TravaloudPermission>(_all.Where(p => p.IsPropertyManager).ToArray());
}

public record TravaloudPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false, bool IsTourManager = false, bool IsPropertyManager = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}