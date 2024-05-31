using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Dashboard;

public class StatsDto
{
    public int BookingsCount { get; init; }
    public int TourBookingsCount { get; init; }
    public decimal TourBookingsRevenue { get; init; }
    public int PropertyBookingsCount { get; init; }
    public int GuestsCount { get; init; }
    public int ToursCount { get; init; }
    public int PropertiesCount { get; init; }
    public List<BookingExportDto>? TodaysTours { get; set; }
    
    public List<ChartSeries> DataEnterBarChart { get; set; } = [];
    public Dictionary<string, double>? ProductByBrandTypePieChart { get; set; }
    
    public IEnumerable<BookingItemDetailsDto>? PaidTourBookings { get; init; }
    public IEnumerable<BookingItemDetailsDto>? AllTourBookings { get; init; }

    public List<TourBookingsBarChartSummary> TourBookingsBarChartSummaries { get; set; } = [];

    public GetDashboardResponse CloudbedsDashboard { get; set; } = new();
}

public class ChartSeries
{
    public string? Name { get; set; }
    public double[]? Data { get; set; }
}

public class TourBookingsBarChartSummary
{
    public string? TourName { get; set; }
    public List<MonthAmount> MonthlyAmounts { get; set; } = [];

    public class MonthAmount
    {
        public DateTime MonthYear { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}