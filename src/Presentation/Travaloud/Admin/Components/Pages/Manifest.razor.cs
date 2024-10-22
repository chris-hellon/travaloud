using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Dashboard;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Manifest : ComponentBase
{
    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    [CascadingParameter] private MudTheme? CurrentTheme { get; set; }
    
    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    [Inject] private IBookingsService BookingsService { get; set; } = default!;
    [Inject] private IToursService ToursService { get; set; } = default!;
    
    private ICollection<TourDto>? Tours { get; set; } = new List<TourDto>();
    private ICollection<TimeSpan>? SelectedTourTimes { get; set; }
    
    private EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>? Context
    {
        get;
        set;
    }
    
    private EntityTable<BookingExportDto, DefaultIdType, BookingExportDto> _table = new();

    protected override void OnInitialized()
    {
        var toursTask = Task.Run(() =>
            ToursService.SearchAsync(new SearchToursRequest {PageNumber = 1, PageSize = 99999}));

        Tours = toursTask.Result?.Data;

        Context = new EntityServerTableContext<BookingExportDto, DefaultIdType, BookingExportDto>(
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
                new EntityField<BookingExportDto>(
                    GetBookingStatus, L["Status"], "IsPaid",
                    Color: GetBookingColor),
                new EntityField<BookingExportDto>(booking => booking.WaiverSigned, L["Waiver Signed"],
                    "BookingWaiverSigned"),
            ],
            enableAdvancedSearch: false,
            createAction: string.Empty,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            viewAction: string.Empty,
            searchFunc: async (filter) =>
            {
                var adaptedFilter = filter.Adapt<GetBookingItemsByDateRequest>();
                adaptedFilter.TourStartDate = DateTime.Now;
                adaptedFilter.TourEndDate = DateTime.Now;
                adaptedFilter.TourId = SearchTourId;
                // adaptedFilter.Description = SearchDescription;
                adaptedFilter.TourStartTime = SearchTourStartTime;
                adaptedFilter.TenantId = TenantInfo.Id;
                adaptedFilter.ApplyPagination = false;
 
                var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
                    adaptedFilter);

                if (SearchTourId.HasValue && SearchTourStartTime == null)
                {
                    SelectedTourTimes = todaysTours.Data.Select(x => x.StartDate.TimeOfDay).Distinct().ToList();
                }
                
                var firstLoad = !SearchTourId.HasValue && string.IsNullOrEmpty(SearchDescription) && !SearchTourStartTime.HasValue;
                
                if (todaysTours.Data.Count != 0 && firstLoad)
                {
                    var tourIds = todaysTours.Data.Select(x => x.TourId).Distinct();
                    Tours = Tours?.Where(x => tourIds.Contains(x.Id)).ToList();
                }

                adaptedFilter.ApplyPagination = true;
                var paginatedTodaysTours = adaptedFilter.Paginate(todaysTours.Data);
                todaysTours.Data = paginatedTodaysTours;

                return todaysTours.Adapt<PaginationResponse<BookingExportDto>>();
            },
            exportFunc: async (filter) =>
            {
                var adaptedFilter = new ExportBookingsByDapperRequest()
                {
                    TourStartDate = DateTime.Now,
                    TourEndDate = DateTime.Now,
                    TourId = SearchTourId,
                    Description = SearchDescription,
                    TenantId = TenantInfo.Id,
                };
                return await BookingsService.ExportAsync(adaptedFilter);
            }
        );
    }

    private async Task CheckGuestIn(BookingExportDto guest)
    {
        await ServiceHelper.ExecuteCallGuardedAsync(async () =>
            {
                await BookingsService.UpdateBookingItemGuestStatus(new UpdateBookingItemGuestStatusRequest(
                    guest.BookingInvoiceId,
                    guest.TourId.Value,
                    guest.StartDate,
                    guest.GuestId,
                    guest.TourDateId,
                    true,
                    false,
                    false
                    ));

                await _table.ReloadDataAsync();
            },
            Snackbar,
            Logger,
            "Guest Checked In Successfully");
    }
    
    private async Task FlagGuestAsNoShow(BookingExportDto guest)
    {
        await ServiceHelper.ExecuteCallGuardedAsync(async () =>
            {
                await BookingsService.UpdateBookingItemGuestStatus(new UpdateBookingItemGuestStatusRequest(
                    guest.BookingInvoiceId,
                    guest.TourId.Value,
                    guest.StartDate,
                    guest.GuestId,
                    guest.TourDateId,
                    false,
                    true,
                    false
                ));
                
                await _table.ReloadDataAsync();
            },
            Snackbar,
            Logger,
            "Guest Flagged as No Show Successfully");
    }
    
    private async Task CancelGuest(BookingExportDto guest)
    {
        await ServiceHelper.ExecuteCallGuardedAsync(async () =>
            {
                await BookingsService.UpdateBookingItemGuestStatus(new UpdateBookingItemGuestStatusRequest(
                    guest.BookingInvoiceId,
                    guest.TourId.Value,
                    guest.StartDate,
                    guest.GuestId,
                    guest.TourDateId,
                    false,
                    false,
                    true
                ));
                
                await _table.ReloadDataAsync();
            },
            Snackbar,
            Logger,
            "Guest Cancelled Successfully");
    }
    
    private string GetBookingStatus(BookingExportDto booking)
    {
        return booking.Cancelled.HasValue && booking.Cancelled.Value ? "Cancelled" :
            booking.NoShow.HasValue && booking.NoShow.Value ? "No Show" :
            booking.CheckedIn.HasValue && booking.CheckedIn.Value ? "Checked In" :
            booking.BookingIsPaid ? "Paid" :
            booking.BookingRefunded.HasValue && booking.BookingRefunded.Value ? "Refunded" :
            booking.BookingAmountOutstanding is > 0 ? "Partially Paid" : "Unpaid";
    }

    private MudColor? GetBookingColor(BookingExportDto booking)
    {
        return (booking.Cancelled.HasValue && booking.Cancelled.Value) || (booking.NoShow.HasValue && booking.NoShow.Value) ||
                   !booking.BookingIsPaid && (!booking.BookingRefunded.HasValue || !booking.BookingRefunded.Value)
            ? CurrentTheme?.Palette.Error
            : booking.CheckedIn.HasValue && booking.CheckedIn.Value ?
               new MudColor(Colors.Green.Default)
            : null;
    }
    
    private string? _searchDescription;

    private string? SearchDescription
    {
        get => _searchDescription ?? null;
        set
        {
            _searchDescription = value;
            _ = _table.ReloadDataAsync();
        }
    }
    
    private DefaultIdType? _searchTourId;

    private DefaultIdType? SearchTourId
    {
        get => _searchTourId;
        set
        {
            _searchTourId = value;
            SearchTourStartTime = null;
            _ = _table.ReloadDataAsync();
        }
    }
    
    private TimeSpan? _searchTourStartTime;

    private TimeSpan? SearchTourStartTime
    {
        get => _searchTourStartTime;
        set
        {
            _searchTourStartTime = value;
            _ = _table.ReloadDataAsync();
        }
    }
}