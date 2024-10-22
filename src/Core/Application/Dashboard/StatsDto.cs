using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Dashboard;

public class StatsDto
{
    public int TodaysBookingsCount { get; init; }
    public int TodaysTourDeparturesCount { get; init; }
    public decimal TodaysBookingsRevenue { get; init; }
    public int TotalBookingsCount { get; init; }
    public decimal TotalBookingsRevenue { get; init; }
    public int TotalCancelledCount { get; init; }
    public decimal TotalCancelledRevenue { get; init; }
    public int TotalRefundedCount { get; init; }
    public decimal TotalRefundedRevenue { get; init; }
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