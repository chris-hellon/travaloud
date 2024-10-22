using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Layout;

public partial class NavMenu
{
    [Parameter] public EventCallback<string> CurrentUrl { get; set; }
    
    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;
    
    [Inject]
    protected IHostEnvironment Env { get; set; } = default!;
    
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    private string? _hangfireUrl;
    private bool _canViewDashboard;
    private bool _canViewCalendar;
    private bool _canViewHangfire;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewDestinations;
    private bool _canViewTours;
    private bool _canViewTenants;
    private bool _canViewGuests;
    private bool _canViewPartners;
    private bool _canViewProperties;
    // private bool _canViewBookings;
    
    private bool _canViewTourBookings;
    private bool _canViewPropertyBookings;
    
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;
    private bool _canViewEvents;
    private bool _canViewServices;
    private bool _canViewJobVacancies;
    private bool _canViewEnquiries;
    private bool _canViewTourGroups;
    private bool _canViewPages;
    private bool _canViewTravelGuides;
    private bool _canViewGallery;
    private bool _canViewSettings;
    private bool _canViewSuppliers;
    
    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        
        await RenderPage();
    }

    protected override async Task<Task> OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return base.OnAfterRenderAsync(firstRender);
        
        await RenderPage();
        StateHasChanged();

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task RenderPage()
    {
        _hangfireUrl = "/jobs";
        
        SetCurrentUser();
        
        var authState = await AuthState.GetAuthenticationStateAsync();
        var user = authState.User;
        
        _canViewHangfire = Env.IsDevelopment();
        _canViewDashboard = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Dashboard); 
        _canViewCalendar = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Calendar);
        _canViewRoles = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Users);
        _canViewDestinations = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Locations);
        _canViewTours = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Tours);
        _canViewTenants = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Tenants);
        _canViewGuests = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Guests);
        _canViewPartners = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Suppliers);
        _canViewProperties = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Properties);
        //_canViewBookings = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Bookings);
        _canViewTourBookings = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.TourBookings);
        _canViewPropertyBookings = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.PropertyBookings);
        _canViewEvents = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Events);
        _canViewServices = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Services);
        _canViewJobVacancies = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.JobVacancies);
        _canViewEnquiries = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Enquiries);
        _canViewPages = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Pages);
        _canViewTravelGuides = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.TravelGuides);
        _canViewGallery = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Gallery);
        _canViewSettings = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Settings);
        _canViewSuppliers = await AuthService.HasPermissionAsync(user, TravaloudAction.View, TravaloudResource.Suppliers);
        _canViewTourGroups = TenantInfo?.Id switch
        {
            "fuse" => false,
            "uncut" => true,
            "vbh" => true,
            _ => _canViewTourGroups
        };
    }
    
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        LoadingService.ToggleLoaderVisibility(true);
        
        SetCurrentUser();
        
        LoadingService.ToggleLoaderVisibility(false);
        
        StateHasChanged();
    }

    private void SetCurrentUser()
    {
        var currentUser = AuthState.GetAuthenticationStateAsync().Result;
        CurrentUserInitializer.SetCurrentUser(currentUser.User);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    private bool ShouldExpand(string prefix)
    {
        var path = (new Uri(NavigationManager.Uri)).PathAndQuery;
        return path.StartsWith(prefix);
    }
}