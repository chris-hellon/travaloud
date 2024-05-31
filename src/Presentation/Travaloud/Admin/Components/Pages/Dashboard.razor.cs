using ApexCharts;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
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
using Travaloud.Application.Common.Models;
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
    [Parameter] public int? FutureTourBookingsCount { get; set; }
    [Parameter] public int? TodayTourBookingsCount { get; set; }
    [Parameter] public int? TodayTourDeparturesCount { get; set; }
    [Parameter] public decimal? TodayTourBookingsRevenue { get; set; }
    [Parameter] public decimal? TourBookingsRevenue { get; set; }
    [Parameter] public int? PropertyBookingsCount { get; set; }
    [Parameter] public int? ToursCount { get; set; }
    [Parameter] public int? GuestsCount { get; set; }
    [Parameter] public IEnumerable<BookingItemDetailsDto>? PaidTourBookings { get; set; }
    [Parameter] public IEnumerable<BookingItemDetailsDto>? AllTourBookings { get; set; } 
    [Parameter] public List<TourBookingsBarChartSummary>? TourBookingsBarChartSummaries { get; set; }
    
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    [CascadingParameter] private MudTheme? CurrentTheme { get; set; }
    
    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    [Inject] private IBookingsService BookingsService { get; set; } = default!;
    [Inject] private IBackgroundJobsService BackgroundJobsService { get; set; } = default!;
    [Inject] private IToursService ToursService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    
    private ICollection<TourDto> Tours { get; set; } = new List<TourDto>();
    private List<UserDetailsDto>? Guests { get; set; }
    
    private bool UserIsAdmin { get; set; }
    private bool UserIsSupplier { get; set; }

    private EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>? TodaysDeparturesContext { get; set; }
    private EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>? TomorrowsDeparturesContext { get; set; }
    private EntityServerTableContext<BookingDto, DefaultIdType, BookingDto>? TodaysActivityContext { get; set; }
    private EntityServerTableContext<StaffBookingDto, string, StaffBookingDto>? StaffBookingsContext { get; set; }
    
    private EntityTable<BookingExportDto, DefaultIdType, BookingExportDto> _todaysDeparturesTable = new();
    private EntityTable<BookingExportDto, DefaultIdType, BookingExportDto> _tomorrowsDeparturesTable = new();
    private EntityTable<BookingDto, DefaultIdType, BookingDto> _todaysActivityTable = new();
    private EntityTable<StaffBookingDto, string, StaffBookingDto> _staffBookingsTable = new();

    private ApexChart<TourBookingsBarChartSummary.MonthAmount> _tourBookingsBarChart = new();
    private ApexChart<BookingItemDetailsDto> _toursRevenuePieChart = new();
    private ApexChartOptions<BookingItemDetailsDto>? _toursRevenuePieChartOptions;
    private ApexChartOptions<TourBookingsBarChartSummary.MonthAmount>? _tourBookingsBarChartOptions;
    
    private MudDateRangePicker _staffBookingsDateRangePicker = new();
    
    protected override void OnInitialized()
    {        
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
        
        var toursTask = Task.Run(() => ToursService.SearchAsync(new SearchToursRequest {PageNumber = 1, PageSize = 99999}));
        var guestsTask = Task.Run(() => UserService.SearchByDapperAsync(new SearchByDapperRequest()
        {
            PageNumber = 1,
            PageSize = int.MaxValue,
            TenantId = TenantInfo?.Id!,
            Role = TravaloudRoles.Guest
        }, CancellationToken.None));

        Tours = toursTask.Result.Data;
        Guests = guestsTask.Result.Data;
        
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
        UserIsAdmin = authState.User.IsInRole(TravaloudRoles.Admin);
        UserIsSupplier = authState.User.IsInRole(TravaloudRoles.Supplier);
    }

    private async Task LoadData()
    {
        var statsRequest = await DashboardService.GetAsync(new GetStatsRequest()
        {
            TenantId = TenantInfo.Id,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now
        });

        PropertiesCount = statsRequest.PropertiesCount;
        ToursCount = statsRequest.ToursCount;
        GuestsCount = statsRequest.GuestsCount;
        BookingsCount = statsRequest.BookingsCount;
        TourBookingsCount = statsRequest.TourBookingsCount;
        TourBookingsRevenue = statsRequest.TourBookingsRevenue;
        PropertyBookingsCount = statsRequest.PropertyBookingsCount;
        AllTourBookings = statsRequest.AllTourBookings;
        PaidTourBookings = statsRequest.PaidTourBookings;
        TourBookingsBarChartSummaries = statsRequest.TourBookingsBarChartSummaries;

        var todaysBookings =
            (PaidTourBookings ?? Array.Empty<BookingItemDetailsDto>()).Where(x =>
                x.CreatedOn.Date == DateTime.Now.Date);

        var bookingItemDetailsDtos = todaysBookings as BookingItemDetailsDto[] ?? todaysBookings.ToArray();
        TodayTourBookingsCount = bookingItemDetailsDtos.Length;
        TodayTourBookingsRevenue = bookingItemDetailsDtos.Sum(x => x.TotalAmount);
    }

    private void LoadTables()
    {
        TodaysDeparturesContext = new EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>(
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
                new EntityField<BookingExportDto>(booking => booking.BookingIsPaid ? "Paid" : booking.BookingRefunded.HasValue && booking.BookingRefunded.Value ? "Refunded" : booking.BookingAmountOutstanding.HasValue ? "Partially Paid" : "Unpaid", L["Status"], "IsPaid",
                    Color: booking => !booking.BookingIsPaid && (!booking.BookingRefunded.HasValue || !booking.BookingRefunded.Value) ? CurrentTheme?.Palette.Error : null),
                new EntityField<BookingExportDto>(booking => booking.WaiverSigned, L["Waiver Signed"], "BookingWaiverSigned"),
            ],
            enableAdvancedSearch: false,
            createAction: string.Empty,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<GetBookingItemsByDateRequest>();
                adaptedFilter.Guests = Guests;
                adaptedFilter.TourStartDate = DateTime.Now;
                adaptedFilter.TourEndDate = DateTime.Now;
                adaptedFilter.TourId = SearchTourId;
                adaptedFilter.Description = SearchDescription;

                var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
                    adaptedFilter);

                TodayTourDeparturesCount = todaysTours.TotalCount;
                return todaysTours.Adapt<EntityTable.PaginationResponse<BookingExportDto>>();
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

        TomorrowsDeparturesContext = new EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>(
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
                new EntityField<BookingExportDto>(booking => booking.BookingIsPaid ? "Paid" : booking.BookingRefunded.HasValue && booking.BookingRefunded.Value ? "Refunded" : booking.BookingAmountOutstanding.HasValue ? "Partially Paid" : "Unpaid", L["Status"], "IsPaid",
                    Color: booking => !booking.BookingIsPaid && (!booking.BookingRefunded.HasValue || !booking.BookingRefunded.Value) ? CurrentTheme.Palette.Error : null),
                new EntityField<BookingExportDto>(booking => booking.WaiverSigned, L["Waiver Signed"], "BookingWaiverSigned"),
            ],
            enableAdvancedSearch: false,
            createAction: string.Empty,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<GetBookingItemsByDateRequest>();
                adaptedFilter.Guests = Guests;
                adaptedFilter.TourStartDate = DateTime.Now.AddDays(1);
                adaptedFilter.TourEndDate = DateTime.Now.AddDays(1);
                adaptedFilter.TourId = SearchTomorrowTourId;
                adaptedFilter.Description = SearchTomorrowDescription;

                var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
                    adaptedFilter);

                return todaysTours.Adapt<EntityTable.PaginationResponse<BookingExportDto>>();
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
        
        TodaysActivityContext = new EntityServerTableContext<BookingDto, DefaultIdType, BookingDto>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingDto>(booking => booking.InvoiceId, L["Reference"], "InvoiceId"),
                new EntityField<BookingDto>(booking => booking.Description, L["Description"], "Description"),
                new EntityField<BookingDto>(booking => booking.GuestName, L["Guest"], "GuestName"),
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate"),
                new EntityField<BookingDto>(booking => $"$ {booking.TotalAmount:n2}", L["Total Amount"], "TotalAmount"),
                new EntityField<BookingDto>(booking => booking.IsPaid ? "Paid" : booking.Refunded.HasValue && booking.Refunded.Value ? "Refunded" : booking.AmountOutstanding.HasValue ? "Partially Paid" : "Unpaid", L["Status"], "IsPaid",
                    Color: booking => !booking.IsPaid && (!booking.Refunded.HasValue || !booking.Refunded.Value) ? CurrentTheme.Palette.Error : null)
            ],
            enableAdvancedSearch: false,
            searchFunc: async filter => await SearchTourBookings(filter),
            createAction: string.Empty,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            exportAction: string.Empty
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
                adaptedFilter.FromDate = SearchStaffBookingsDateRange.Start.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
                adaptedFilter.ToDate = SearchStaffBookingsDateRange.End.Value.Date + new TimeSpan(0, 23, 59, 59, 999);

                var staffBookings = await BookingsService.StaffBookingsByDateRange(adaptedFilter);
                return staffBookings.Adapt<EntityTable.PaginationResponse<StaffBookingDto>>();
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
    
    public void HandleOnTodayTourDeparturesCountSet(int departuresCount)
    {
        TodayTourDeparturesCount = departuresCount;
    }
    
    private async Task UpdateDashboard()
    {
        await Task.WhenAll(LoadData(),
            _staffBookingsTable.ReloadDataAsync(),
            _todaysDeparturesTable.Context != null ? _todaysDeparturesTable.ReloadDataAsync() : Task.CompletedTask,
            _todaysActivityTable.ReloadDataAsync(),
            _tourBookingsBarChart.RenderAsync(),
            _toursRevenuePieChart.RenderAsync(),
            _tomorrowsDeparturesTable.Context != null ? _tomorrowsDeparturesTable.ReloadDataAsync() : Task.CompletedTask);

        await InvokeAsync(StateHasChanged);
    }
    private void ShowStaffReportDetails(StaffBookingDto request)
    {
        request.ShowDetails = !request.ShowDetails;
    }
    #region Search Fields
    private async Task<EntityTable.PaginationResponse<BookingDto>> SearchTourBookings(PaginationFilter filter)
    {
        var adaptedFilter = filter.Adapt<SearchBookingsRequest>();
        adaptedFilter.IsTours = true;
        adaptedFilter.TourId = SearchTourId;
        adaptedFilter.BookingStartDate = DateTime.Now;
        adaptedFilter.BookingEndDate = DateTime.Now;

        if (adaptedFilter.BookingEndDate.HasValue)
        {
            adaptedFilter.BookingEndDate =
                adaptedFilter.BookingEndDate.Value.Date + new TimeSpan(0, 23, 59, 59, 999);
        }

        if (adaptedFilter.BookingStartDate.HasValue)
        {
            adaptedFilter.BookingStartDate =
                adaptedFilter.BookingStartDate.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
        }

        if (adaptedFilter is {TourEndDate: not null, TourStartDate: not null})
        {
            adaptedFilter.TourEndDate = adaptedFilter.TourEndDate.Value.Date + new TimeSpan(0, 23, 59, 59, 999);
            adaptedFilter.TourStartDate = adaptedFilter.TourStartDate.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
        }

        var request = await BookingsService.SearchAsync(adaptedFilter);

        var staffIds = request.Data.Select(x => x.CreatedBy.ToString()).ToList();
        var primaryGuestIds = request.Data.Where(x => !x.GuestId.IsNullOrEmpty()).Select(x => x.GuestId).ToList();
        var additionalGuestIds = request.Data
            .Where(x => x.Items != null && x.Items.Any(i => i.Guests is {Count: > 0}))
            .SelectMany(x => x.Items.Where(i => i.Guests.Count > 0)
                .SelectMany(i => i.Guests.Select(g => g.GuestId.ToString()))).ToList();

        var allUserIds = staffIds.Concat(primaryGuestIds).Concat(additionalGuestIds).ToList();

        var users = await UserService.SearchAsync(allUserIds, CancellationToken.None);

        if (users.Count == 0) return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
        {
            var bookings = request.Data.Select(x =>
            {
                var staffMember = users.FirstOrDefault(s => s.Id == x.CreatedBy);

                if (staffMember != null)
                    x.StaffName = $"{staffMember.FirstName} {staffMember.LastName}";

                var primaryGuest = users.FirstOrDefault(u => u.Id == DefaultIdType.Parse(x.GuestId));
                if (primaryGuest != null)
                {
                    x.PrimaryGuest = primaryGuest;
                }

                if (x.Items != null)
                    x.Items = x.Items.Select(i =>
                    {
                        if (i.Guests != null)
                            i.Guests = i.Guests.Select(g =>
                            {
                                var guest = users.FirstOrDefault(u => u.Id == g.GuestId);

                                if (guest == null) return g;

                                g.FullName = guest.FullName ?? $"{guest.FirstName} {guest.LastName}";
                                g.DateOfBirth = guest.DateOfBirth;
                                g.Nationality = guest.Nationality;
                                g.Email = guest.Email;
                                g.Gender = guest.Gender;

                                return g;
                            }).ToList();

                        return i;
                    }).ToList();

                return x;
            });

            request.Data = bookings.ToList();
        }

        return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
    }
    
    private string? _searchDescription;

    private string SearchDescription
    {
        get => _searchDescription ?? string.Empty;
        set
        {
            _searchDescription = value;
            _ = _todaysDeparturesTable.ReloadDataAsync();
        }
    }

    private string? _searchTomorrowDescription;

    private string SearchTomorrowDescription
    {
        get => _searchTomorrowDescription ?? string.Empty;
        set
        {
            _searchTomorrowDescription = value;
            _ = _tomorrowsDeparturesTable.ReloadDataAsync();
        }
    }

    private DefaultIdType? _searchTourId;

    private DefaultIdType? SearchTourId
    {
        get => _searchTourId;
        set
        {
            _searchTourId = value;
            _ = _todaysDeparturesTable.ReloadDataAsync();
        }
    }

    private DefaultIdType? _searchTomorrowTourId;

    private DefaultIdType? SearchTomorrowTourId
    {
        get => _searchTomorrowTourId;
        set
        {
            _searchTomorrowTourId = value;
            _ = _tomorrowsDeparturesTable.ReloadDataAsync();
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
    #endregion
}