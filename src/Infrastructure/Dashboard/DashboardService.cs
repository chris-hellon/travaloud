using MediatR;
using Travaloud.Application.Dashboard;
using Travaloud.Infrastructure.Catalog.Services;

namespace Travaloud.Infrastructure.Dashboard;

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(ISender mediator) : base(mediator)
    {
    }

    public Task<StatsDto> GetAsync()
    {
        return Mediator.Send(new GetStatsRequest());
    }
}