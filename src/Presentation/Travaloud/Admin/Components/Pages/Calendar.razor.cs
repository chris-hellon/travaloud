using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages;

public partial class Calendar
{
    [Inject] protected IDashboardService DashboardService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private PersistentComponentState ApplicationState { get; set; }

    private PersistingComponentStateSubscription _subscription;
    
    private List<CalendarItem> _events = new();

    private async Task DateRangeChanged(DateRange dateRange)
    {
        var adaptedFilter = new GetBookingItemsByDateRequest();
        adaptedFilter.TourStartDate = dateRange.Start.Value;
        adaptedFilter.TourEndDate = dateRange.End.Value;
        adaptedFilter.PageNumber = 1;
        adaptedFilter.PageSize = 99999;
        adaptedFilter.Guests = await UserService.GetListAsync(TravaloudRoles.Guest);;
        
        var todaysTours = await DashboardService.GetTourBookingItemsByDateAsync(
            adaptedFilter);

        var todaysToursGrouped = todaysTours.Data.GroupBy(x => $"{x.TourName}|{x.StartDate.ToLongDateString()}{x.EndDate.ToShortDateString()}")
            .Select(x => new
            {
                TourName = x.Key.Split('|')[0],
                Tours = x.ToList()
            });
        
        _events = todaysToursGrouped.Select(x => new CalendarItem()
        {
            Start = x.Tours.First().StartDate,
            End = x.Tours.First().EndDate,
            Text = $"{x.TourName}|{x.Tours.Count} Guest{(x.Tours.Count > 1 ? "s" : "")}"
            //Text = $"{x.TourName}|{string.Join(", ", x.Tours.Select(t => t.GuestName))}"
        }).ToList();
    }
}