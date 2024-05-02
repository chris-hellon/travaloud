using MediatR;
using Travaloud.Application.Catalog.Bookings.Dto;
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

    public Task<StatsDto> GetAsync(List<UserDetailsDto> guests)
    {
        return Mediator.Send(new GetStatsRequest()
        {
            Guests = guests
        });
    }
    
    public Task<PaginationResponse<BookingExportDto>> GetTourBookingItemsByDateAsync(GetBookingItemsByDateRequest request)
    {
        return Mediator.Send(request);
    }
}