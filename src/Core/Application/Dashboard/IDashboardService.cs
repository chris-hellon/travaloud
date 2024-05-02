using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Identity.Users;

namespace Travaloud.Application.Dashboard;

public interface IDashboardService : ITransientService
{
    Task<StatsDto> GetAsync(List<UserDetailsDto> guests);
    Task<PaginationResponse<BookingExportDto>> GetTourBookingItemsByDateAsync(GetBookingItemsByDateRequest request);
}