using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs.Calendar;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Calendar
{
    [Inject] protected IDashboardService DashboardService { get; set; } = default!;
    [Inject] protected IToursService ToursService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    protected List<CalendarItem> CalendarItems = [];
    private List<BookingExportDto>? BookingExports { get; set; }
    private IList<CalendarDto>? _calendarItems { get; set; }
    
    private async Task DateRangeChanged(DateRange dateRange)
    {

        _calendarItems =
            await DashboardService.GetCalendar(new GetCalendarRequest(TenantInfo?.Id, dateRange.Start.Value,
                dateRange.End.Value));

        CalendarItems = _calendarItems.Select(x => new CalendarItem()
        {
            Start = x.StartDate,
            End = x.StartDate,
            Text = $"{x.Tour}|{x.GuestCount} Guest{(x.GuestCount> 1 ? "s" : "")}|{x.TourId}|{x.StartDate:hh:mm tt} - {x.EndDate:hh:mm tt}|{x.StartDate}|" 
        }).ToList();
    }
    
    private async Task OnItemClick(CalendarItem item)
    {
        var tourName = item.Text.Split('|')[0];
        var tourId = item.Text.Split('|')[2];
        var startDateString = item.Text.Split('|')[4];
        var startDate = DateTime.Parse(startDateString);
        
        var adaptedFilter = new GetCalendarItemsRequest()
        {
            TourStartDate = startDate,
            TourEndDate =startDate,
            TourStartTime = startDate.TimeOfDay,
            TenantId = TenantInfo.Id,
            TourId = DefaultIdType.Parse(tourId),
            PaidOnly = true
        };
        
        var bookingExports = await DashboardService.GetCalendarItems(
                 adaptedFilter);
        
        // var bookingExports = BookingExports?.Where(x => x.TourId == DefaultIdType.Parse(tourId) && x.StartDate == item.Start).ToList();
        //
        
        var exportRequest = new GetBookingItemsByDateRequest
        {
            TourStartDate = startDate,
            TourEndDate =startDate,
            TourStartTime = startDate.TimeOfDay,
            PageNumber = 1,
            PageSize = 99999,
            TenantId = TenantInfo.Id,
            TourId = DefaultIdType.Parse(tourId),
            PaidOnly = true
        };
        
        var parameters = new DialogParameters
        {
            {nameof(GuestsDialog.ExportRequest), exportRequest},
            {nameof(GuestsDialog.BookingExports), bookingExports},
            {nameof(GuestsDialog.StartDate), item.Start},
            {nameof(GuestsDialog.EndDate), item.End},
            {nameof(GuestsDialog.TourName), tourName},
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        
        var dialog = await DialogService.ShowAsync<GuestsDialog>(L[$"Guests for {tourName} {item.Start.ToShortDateString()} {item.Start.TimeOfDay}"], parameters, options);
        
        await dialog.Result;
        
    }
}