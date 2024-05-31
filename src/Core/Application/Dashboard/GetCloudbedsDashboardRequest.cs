
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Dashboard;

public class GetCloudbedsDashboardRequest : IRequest<GetDashboardResponse>
{
    public DateTime? Date { get; set; }
}

internal class GetCloudbedsDashboardRequestHandler : IRequestHandler<GetCloudbedsDashboardRequest, GetDashboardResponse>
{
    private readonly ICloudbedsService _cloudbedsService;
    private readonly IRepositoryFactory<Property> _propertiesRepo;
    
    public GetCloudbedsDashboardRequestHandler(ICloudbedsService cloudbedsService, IRepositoryFactory<Property> propertiesRepo)
    {
        _cloudbedsService = cloudbedsService;
        _propertiesRepo = propertiesRepo;
    }

    public async Task<GetDashboardResponse> Handle(GetCloudbedsDashboardRequest request, CancellationToken cancellationToken)
    {
        var properties = await _propertiesRepo.ListAsync(cancellationToken);
    
        var dashboards = await _cloudbedsService.GetDashboards(
            properties.Select(x => new GetDashboardRequest(x.CloudbedsPropertyId, x.CloudbedsApiKey)));

        // Initialize totals
        double totalRooms = 0;
        double totalOccupied = 0;

        // Calculate the total number of occupied rooms and total rooms
        foreach (var dashboard in dashboards)
        {
            double roomsOccupied = dashboard.RoomsOccupied;
            double percentageOccupied = dashboard.PercentageOccupied;

            // Calculate total rooms for the property
            var totalRoomsProperty = roomsOccupied / (percentageOccupied / 100);

            totalRooms += totalRoomsProperty;
            totalOccupied += roomsOccupied;
        }

        // Calculate the overall percentage occupied
        var overallPercentageOccupied = (totalOccupied / totalRooms) * 100;

        return new GetDashboardResponse()
        {
            Arrivals = dashboards.Sum(x => x.Arrivals),
            Departures = dashboards.Sum(x => x.Departures),
            PercentageOccupied = overallPercentageOccupied, // Use the calculated overall percentage occupied
            InHouse = dashboards.Sum(x => x.InHouse),
            RoomsOccupied = dashboards.Sum(x => x.RoomsOccupied)
        };
    }

}