using Microsoft.AspNetCore.Components;
using Travaloud.Application.Dashboard;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Pages;

public partial class Dashboard
{
    [Parameter] public int PropertiesCount { get; set; }
    [Parameter] public int BookingsCount { get; set; }
    [Parameter] public int ToursCount { get; set; }
    [Parameter] public int GuestsCount { get; set; }

    [Inject] private IDashboardService DashboardService { get; set; } = default!;
    
    private readonly string[] _dataEnterBarChartXAxisLabels =
        {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

    private readonly List<MudBlazor.ChartSeries> _dataEnterBarChartSeries = new();
    private bool _loaded;

    protected override async Task<Task> OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return base.OnAfterRenderAsync(firstRender);
        
        await LoadDataAsync();

        _loaded = true;

        StateHasChanged();
        
        return base.OnAfterRenderAsync(firstRender);
    }
    //
    // protected override async Task OnInitializedAsync()
    // {
    //     await LoadDataAsync();
    //
    //     _loaded = true;
    // }

    private async Task LoadDataAsync()
    {
        if (await ServiceHelper.ExecuteCallGuardedAsync(
                () => DashboardService.GetAsync(),
                Snackbar,
                Logger)
            is { } statsDto)
        {
            PropertiesCount = statsDto.PropertiesCount;
            ToursCount = statsDto.ToursCount;
            GuestsCount = statsDto.GuestsCount;
            BookingsCount = statsDto.BookingsCount;

            foreach (var item in statsDto.DataEnterBarChart)
            {
                _dataEnterBarChartSeries
                    .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                _dataEnterBarChartSeries.Add(new MudBlazor.ChartSeries {Name = item.Name, Data = item.Data?.ToArray()});
            }
        }
    }
}