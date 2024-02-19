namespace Travaloud.Application.Dashboard;

public class StatsDto
{
    public int BookingsCount { get; set; }
    public int GuestsCount { get; set; }
    public int ToursCount { get; set; }
    public int PropertiesCount { get; set; }
    public List<ChartSeries> DataEnterBarChart { get; set; } = new();
    public Dictionary<string, double>? ProductByBrandTypePieChart { get; set; }
}

public class ChartSeries
{
    public string? Name { get; set; }
    public double[]? Data { get; set; }
}