using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Dashboard;

public interface IDashboardService : ITransientService
{
    Task<StatsDto> GetAsync(GetStatsRequest request);
    Task<PaginationResponse<BookingExportDto>> GetTourBookingItemsByDateAsync(GetBookingItemsByDateRequest request);
    Task<GetDashboardResponse> GetCloudbedsDashboard(GetCloudbedsDashboardRequest request);
    Task<IList<CalendarDto>> GetCalendar(GetCalendarRequest request);
    Task<IEnumerable<CalendarItemDto>> GetCalendarItems(GetCalendarItemsRequest request);
}