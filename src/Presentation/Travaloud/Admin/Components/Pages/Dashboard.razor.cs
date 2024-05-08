using ApexCharts;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.BackgroundJobs;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Dashboard
{
    [Parameter] public int? PropertiesCount { get; set; }
    [Parameter] public int? BookingsCount { get; set; }
    [Parameter] public int? TourBookingsCount { get; set; }
    [Parameter] public decimal? TourBookingsRevenue { get; set; }
    [Parameter] public int? PropertyBookingsCount { get; set; }
    [Parameter] public int? ToursCount { get; set; }
    [Parameter] public int? GuestsCount { get; set; }
    [Parameter] public IEnumerable<BookingItemDetailsDto>? PaidTourBookings { get; set; }
    [Parameter] public IEnumerable<BookingItemDetailsDto>? AllTourBookings { get; set; }
    [Parameter] public List<TourBookingsBarChartSummary>? TourBookingsBarChartSummaries { get; set; }
    
    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private IBookingsService BookingsService { get; set; } = default!;
    [Inject] private IToursService ToursService { get; set; } = default!;
    [Inject] private IPropertiesService PropertiesService { get; set; } = default!;
    [Inject] private ICloudbedsService CloudbedsService { get; set; } = default!;
    [Inject] private IBackgroundJobsService BackgroundJobsService { get; set; } = default!;

    private EntityServerTableContext<BookingExportDto, Guid, BookingExportDto>? TourDayBookingsContext { get; set; }
    private EntityServerTableContext<StaffBookingDto, string, StaffBookingDto>? StaffBookingsContext { get; set; }

    private readonly System.Timers.Timer _timer = new();
    
    private bool UserIsAdmin { get; set; }

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    [CascadingParameter] private MudTheme? CurrentTheme { get; set; }
    
    private readonly string[] _dataEnterBarChartXAxisLabels =
        {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

    private readonly List<MudBlazor.ChartSeries> _dataEnterBarChartSeries = new();
    private EntityTable<BookingExportDto, Guid, BookingExportDto> _todaysToursTable = default!;
    private EntityTable<StaffBookingDto, string, StaffBookingDto> _staffBookingsTable = default!;
    private ICollection<TourDto> Tours { get; set; } = default!;

    private MudDateRangePicker _staffBookingsDateRangePicker = default!;

    private List<UserDetailsDto>? Guests { get; set; }

    private ApexChart<TourBookingsBarChartSummary.MonthAmount>? _tourBookingsBarChart;
    private ApexChart<BookingItemDetailsDto>? _toursRevenuePieChart;
    private ApexChartOptions<BookingItemDetailsDto>? _toursRevenuePieChartOptions;
    private ApexChartOptions<TourBookingsBarChartSummary.MonthAmount>? _tourBookingsBarChartOptions;
    
    protected override void OnInitialized()
    {        
        // _timer.Interval = 10000; 
        // _timer.Elapsed += OnTimerTick;
        // _timer.Start();

        _searchStaffBookingsDateRange = new DateRange(DateTime.Now.AddMonths(-1), DateTime.Now);
        _toursRevenuePieChartOptions = new ApexChartOptions<BookingItemDetailsDto>
        {
            Theme = new ApexCharts.Theme { Palette = PaletteType.Palette7},
            Yaxis =
            [
                new YAxis
                {
                    Labels = new YAxisLabels
                    {
                        Formatter = @"function (value) {
                    return '$' + Number(value).toLocaleString();}"
                    }
                }
            ]
        };
        _tourBookingsBarChartOptions = new ApexChartOptions<TourBookingsBarChartSummary.MonthAmount>
        {
            Theme = new ApexCharts.Theme
            {
                Palette = PaletteType.Palette7
            }
        };
        
        var guests = Task.Run(() => UserService.SearchByDapperAsync(new SearchByDapperRequest()
        {
            PageNumber = 1,
            PageSize = int.MaxValue,
            TenantId = TenantInfo.Id,
            Role = TravaloudRoles.Guest
        }, CancellationToken.None)).Result;

        Guests = guests.Data;
        
        LoadTables();
    }

    protected override async Task<Task> OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return base.OnAfterRenderAsync(firstRender);


        await LoadData();
        StateHasChanged();

        Logger.Information("Set Background Jobs if not set already");

        var importCloudbedsBackgroundJobTask = BackgroundJobsService.ImportCloudbedsGuests(new ImportCloudbedsGuestsRequest());
        var sendDailyTourManifestTask = BackgroundJobsService.SendDailyTourManifest(new SendDailyTourManifestBatchRequest());
        
        await Task.WhenAll(importCloudbedsBackgroundJobTask, sendDailyTourManifestTask);
        
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        UserIsAdmin = authState.User.IsInRole("Admin");
    }

    private async Task LoadData()
    {
        var statsDto = await DashboardService.GetAsync(Guests);
        
        PropertiesCount = statsDto.PropertiesCount;
        ToursCount = statsDto.ToursCount;
        GuestsCount = statsDto.GuestsCount;
        BookingsCount = statsDto.BookingsCount;
        TourBookingsCount = statsDto.TourBookingsCount;
        TourBookingsRevenue = statsDto.TourBookingsRevenue;
        PropertyBookingsCount = statsDto.PropertyBookingsCount;
        AllTourBookings = statsDto.AllTourBookings;
        PaidTourBookings = statsDto.PaidTourBookings;
        TourBookingsBarChartSummaries = statsDto.TourBookingsBarChartSummaries;

        foreach (var item in statsDto.DataEnterBarChart)
        {
            _dataEnterBarChartSeries.RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
            _dataEnterBarChartSeries.Add(new MudBlazor.ChartSeries {Name = item.Name, Data = item.Data?.ToArray()});
        }
    }

    private void LoadTables()
    {
        TourDayBookingsContext = new EntityServerTableContext<BookingExportDto, Guid, BookingExportDto>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingExportDto>(booking => booking.BookingInvoiceId, L["Reference"],
                    "BookingInvoiceId"),
                new EntityField<BookingExportDto>(booking => booking.TourName, L["Tour"], "TourName"),
                new EntityField<BookingExportDto>(booking => booking.GuestName, L["Guest"], "GuestName"),
                new EntityField<BookingExportDto>(booking => booking.StartDate.TimeOfDay, L["Start Time"], "StartDate"),
                new EntityField<BookingExportDto>(booking => booking.EndDate.TimeOfDay, L["End Time"], "EndDate"),
                new EntityField<BookingExportDto>(booking => booking.BookingIsPaid, L["Is Paid"], "BookingIsPaid", Color: booking => !booking.BookingIsPaid ? CurrentTheme.Palette.Error : null),
                new EntityField<BookingExportDto>(booking => booking.BookingWaiverSigned, L["Waiver Signed"], "BookingWaiverSigned"),
            ],
            enableAdvancedSearch: false,
            createAction: string.Empty,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            searchFunc: async (filter) =>
            {
                var tours = await ToursService.SearchAsync(new SearchToursRequest {PageNumber = 1, PageSize = 99999});
                Tours = tours?.Data!;

                var adaptedFilter = filter.Adapt<GetBookingItemsByDateRequest>();
                adaptedFilter.Guests = Guests;
                adaptedFilter.TourStartDate = DateTime.Now;
                adaptedFilter.TourEndDate = DateTime.Now;
                adaptedFilter.TourId = SearchTourId;
                adaptedFilter.Description = SearchDescription;

                var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
                    adaptedFilter);

                return todaysTours.Adapt<PaginationResponse<BookingExportDto>>();
            },
            exportFunc: async (filter) =>
            {
                var adaptedFilter = new ExportBookingsRequest()
                {
                    Guests = Guests,
                    TourStartDate = DateTime.Now.Date + new TimeSpan(00, 00, 00),
                    TourEndDate = DateTime.Now.Date + new TimeSpan(23, 59, 59),
                    IsTourBookings = true,
                    TourId = SearchTourId,
                    Description = SearchDescription
                };
                return await BookingsService.ExportAsync(adaptedFilter);
            }
        );

        StaffBookingsContext = new EntityServerTableContext<StaffBookingDto, string, StaffBookingDto>(
            entityName: L["Staff"],
            entityNamePlural: L["Staff Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<StaffBookingDto>(user => user.FullName, L["Staff Name"], "FullName"),
                new EntityField<StaffBookingDto>(user => user.BookingsMade, L["Total Bookings Made"], "BookingsMade"),
                new EntityField<StaffBookingDto>(user => user.ItemsCount, L["Total Items Booked"], "ItemsCount"),
                new EntityField<StaffBookingDto>(user => $"$ {user.TotalBookingsAmount:n2}",
                    L["Total Bookings Revenue"], "TotalBookingsAmount"),
                new EntityField<StaffBookingDto>(user => $"$ {user.TotalComission:n2}", L["Total Commission Amount"], "TotalComission")
            ],
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<StaffBookingsByDateRangeRequest>();
                adaptedFilter.TenantId = TenantInfo.Id;
                adaptedFilter.FromDate =
                    SearchStaffBookingsDateRange.Start.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
                adaptedFilter.ToDate = SearchStaffBookingsDateRange.End.Value.Date + new TimeSpan(0, 23, 59, 59, 999);

                var staffBookings = await BookingsService.StaffBookingsByDateRange(adaptedFilter);
                return staffBookings.Adapt<PaginationResponse<StaffBookingDto>>();
            },
            exportFunc: async filter =>
            {
                var exportFilter = filter.Adapt<ExportStaffBookingsRequest>();
                exportFilter.TenantId = TenantInfo.Id;
                exportFilter.FromDate = SearchStaffBookingsDateRange.Start.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
                exportFilter.ToDate = SearchStaffBookingsDateRange.End.Value.Date + new TimeSpan(0, 23, 59, 59, 999);

                return await BookingsService.ExportStaffBookingsAsync(exportFilter);
            },
            createAction: string.Empty,
            updateAction: string.Empty,
            deleteAction: string.Empty,
            viewAction: string.Empty);
    }

    private string? _searchDescription;

    private string SearchDescription
    {
        get => _searchDescription ?? string.Empty;
        set
        {
            _searchDescription = value;
            _ = _todaysToursTable.ReloadDataAsync();
        }
    }

    private Guid? _searchTourId;

    private Guid? SearchTourId
    {
        get => _searchTourId;
        set
        {
            _searchTourId = value;
            _ = _todaysToursTable.ReloadDataAsync();
        }
    }

    private DateRange? _searchStaffBookingsDateRange;

    private DateRange? SearchStaffBookingsDateRange
    {
        get => _searchStaffBookingsDateRange;
        set
        {
            _searchStaffBookingsDateRange = value;
            _ = _staffBookingsTable.ReloadDataAsync();
        }
    }
    
    private async Task UpdateDashboard()
    {
        await Task.WhenAll(LoadData(), _staffBookingsTable.ReloadDataAsync(), _todaysToursTable.ReloadDataAsync(), _tourBookingsBarChart.RenderAsync(), _toursRevenuePieChart.RenderAsync());

        await InvokeAsync(StateHasChanged);
    }
    //
    // private void OnTimerTick(object sender, System.Timers.ElapsedEventArgs e)
    // {
    //     InvokeAsync(UpdateDashboard);
    // }
    //
    // public void Dispose()
    // {
    //     _timer.Stop();
    //     _timer.Dispose();
    // }
}