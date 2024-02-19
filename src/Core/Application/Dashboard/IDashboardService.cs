namespace Travaloud.Application.Dashboard;

public interface IDashboardService : ITransientService
{
    Task<StatsDto> GetAsync();
}