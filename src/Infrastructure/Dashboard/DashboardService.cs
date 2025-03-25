using MediatR;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Cloudbeds.Responses;
using Travaloud.Application.Common.Models;
using Travaloud.Application.Dashboard;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Catalog.Services;

namespace Travaloud.Infrastructure.Dashboard;

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(ISender mediator) : base(mediator)
    {
    }

    public Task<StatsDto> GetAsync(GetStatsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<PaginationResponse<BookingExportDto>> GetTourBookingItemsByDateAsync(GetBookingItemsByDateRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<GetDashboardResponse> GetCloudbedsDashboard(GetCloudbedsDashboardRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<IList<CalendarDto>> GetCalendar(GetCalendarRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<IEnumerable<CalendarItemDto>> GetCalendarItems(GetCalendarItemsRequest request)
    {
        return Mediator.Send(request);
    }
}