using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Components.Dialogs.Calendar;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Calendar
{
    [Inject] protected IDashboardService DashboardService { get; set; } = default!;
    [Inject] protected IToursService ToursService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;

    protected List<CalendarItem> CalendarItems = [];
    private List<BookingExportDto>? BookingExports { get; set; }
    
    private async Task DateRangeChanged(DateRange dateRange)
    {
        var adaptedFilter = new GetBookingItemsByDateRequest
        {
            TourStartDate = dateRange.Start.Value,
            TourEndDate = dateRange.End.Value,
            PageNumber = 1,
            PageSize = 99999,
            Guests = await UserService.GetListAsync(TravaloudRoles.Guest)
        };

        var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
            adaptedFilter);

        BookingExports = todaysTours.Data;
        
        var todaysToursGrouped = todaysTours.Data.GroupBy(x => $"{x.TourName}|{x.StartDate.ToLongDateString()}{x.EndDate.ToShortDateString()}")
            .Select(x => new
            {
                TourName = x.Key.Split('|')[0],
                Tours = x.ToList()
            });
        
        CalendarItems = todaysToursGrouped.Select(x => new CalendarItem()
        {
            Start = x.Tours.First().StartDate,
            End = x.Tours.First().EndDate,
            Text = $"{x.TourName}|{x.Tours.Count} Guest{(x.Tours.Count > 1 ? "s" : "")}|{x.Tours.First().TourId}"
        }).ToList();
    }
    
    private async Task OnItemClick(CalendarItem item)
    {
        var tourId = item.Text.Split('|')[2];
        var tour = await ToursService.GetAsync(DefaultIdType.Parse(tourId));

        var bookingExports = BookingExports?.Where(x => x.TourId == DefaultIdType.Parse(tourId) && x.StartDate == item.Start).ToList();
        
        var parameters = new DialogParameters
        {
            {nameof(GuestsDialog.BookingExports), bookingExports},
            {nameof(GuestsDialog.StartDate), item.Start},
            {nameof(GuestsDialog.EndDate), item.End},
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        
        var dialog = await DialogService.ShowAsync<GuestsDialog>(L[$"Guests for {tour.Name} {item.Start.TimeOfDay}"], parameters, options);

        await dialog.Result;
    }
}