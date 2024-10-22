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
        // var adaptedFilter = new GetBookingItemsByDateRequest
        // {
        //     TourStartDate = dateRange.Start.Value,
        //     TourEndDate = dateRange.End.Value,
        //     PageNumber = 1,
        //     PageSize = 99999,
        //     TenantId = TenantInfo.Id
        // };

        // var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
        //     adaptedFilter);

        _calendarItems =
            await DashboardService.GetCalendar(new GetCalendarRequest(TenantInfo?.Id, dateRange.Start.Value,
                dateRange.End.Value));

        CalendarItems = _calendarItems.Select(x => new CalendarItem()
        {
            Start = x.StartDate,
            End = x.StartDate,
            Text = $"{x.Tour}|{x.GuestCount} Guest{(x.GuestCount> 1 ? "s" : "")}|{x.TourId}|{x.StartDate:hh:mm tt} - {x.EndDate:hh:mm tt}|{x.StartDate}" 
        }).ToList();
        
        // BookingExports = todaysTours.Data;
        //
        // var todaysToursGrouped = todaysTours.Data.GroupBy(x => $"{x.TourName}|{x.StartDate.ToLongDateString()}{x.EndDate.ToShortDateString()}")
        //     .Select(x => new
        //     {
        //         TourName = x.Key.Split('|')[0],
        //         Tours = x.ToList()
        //     });
        
        // CalendarItems = todaysToursGrouped.Select(x => new CalendarItem()
        // {
        //     Start = x.Tours.First().StartDate,
        //     End = x.Tours.First().StartDate,
        //     Text = $"{x.TourName}|{x.Tours.Count} Guest{(x.Tours.Count > 1 ? "s" : "")}|{x.Tours.First().TourId}|{x.Tours.First().StartDate:hh:mm tt} - {x.Tours.First().EndDate:hh:mm tt}"
        // }).ToList();
    }
    
    private async Task OnItemClick(CalendarItem item)
    {
        var tourName = item.Text.Split('|')[0];
        var tourId = item.Text.Split('|')[2];
        var startDateString = item.Text.Split('|')[4];
        var startDate = DateTime.Parse(startDateString);
        
        var adaptedFilter = new GetBookingItemsByDateRequest
        {
            TourStartDate = startDate,
            TourEndDate =startDate,
            TourStartTime = startDate.TimeOfDay,
            PageNumber = 1,
            PageSize = 99999,
            TenantId = TenantInfo.Id,
            TourId = DefaultIdType.Parse(tourId)
        };
        
        var bookingExports = await DashboardService.GetTourBookingItemsByDateAsync(
                 adaptedFilter);
        
        // var bookingExports = BookingExports?.Where(x => x.TourId == DefaultIdType.Parse(tourId) && x.StartDate == item.Start).ToList();
        //
        var parameters = new DialogParameters
        {
            {nameof(GuestsDialog.BookingExports), bookingExports.Data},
            {nameof(GuestsDialog.StartDate), item.Start},
            {nameof(GuestsDialog.EndDate), item.End},
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        
        var dialog = await DialogService.ShowAsync<GuestsDialog>(L[$"Guests for {tourName} {item.Start.ToShortDateString()} {item.Start.TimeOfDay}"], parameters, options);

        await dialog.Result;
    }
}