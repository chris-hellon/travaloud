using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Org.BouncyCastle.Ocsp;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Common.Utils;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class TourDates : ComponentBase
{
    [Parameter] [EditorRequired] public TourDateRequest RequestModel { get; set; } = default!;

    [Parameter] public TourViewModel Tour { get; set; } = default!;

    [Parameter] public EntityServerTableContext<TourDto, DefaultIdType, TourViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    private DateRange? DateRange { get; set; }

    private DefaultIdType AllPricesId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(RequestModel);
        AllPricesId = DefaultIdType.NewGuid();
        
        await base.OnInitializedAsync();
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (DateRange != null)
        {
            RequestModel.StartDate = DateRange.Start;
        }

        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        Tour.TourDates ??= new List<TourDateRequest>();

                        if (RequestModel.TourPriceId.HasValue && RequestModel.TourPriceId.Value != AllPricesId)
                            AddPrices(RequestModel.TourPriceId);
                        else
                        {
                            if (Tour.TourPrices == null) return Task.CompletedTask;
                            
                            foreach (var price in Tour.TourPrices)
                                AddPrices(price.Id);
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Tour Dates Created. Please note this is not final, you must Save the Tour to confirm these Dates."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }

        await LoadingService.ToggleLoaderVisibility(false);
    }

    private void AddPrices(DefaultIdType? tourPriceId)
    {
        var datePrice = Tour.TourPrices?.FirstOrDefault(x => x.Id == tourPriceId);

        if (datePrice == null || DateRange == null || !DateRange.Start.HasValue ||
            !DateRange.End.HasValue) return;
        {
            var dates = GetDatesInRange(DateRange.Start.Value, DateRange.End.Value);

            const int yearLimit = 1;
            const int recordLimit = 500;

            for (var i = 0; i < dates.Count; i++)
            {
                var date = dates[i];
                var startDate = date.Date + RequestModel.StartTime;

                if (!startDate.HasValue) continue;

                if (startDate > DateTime.Now.AddYears(yearLimit))
                {
                    Snackbar.Add(
                        $"Dates greater than {DateTime.Now.AddYears(yearLimit).ToShortDateString()} were unable to be added.",
                        Severity.Info);
                    break;
                }

                if (i + 1 >= recordLimit)
                {
                    Snackbar.Add(
                        $"A maximum of {recordLimit} dates can be added per request. Any dates from {startDate.Value.Date.ToShortDateString()} onwards have not been added.",
                        Severity.Info);
                    break;
                }

                var startTime = startDate.Value.TimeOfDay;

                var endDate = DateTimeUtils.CalculateEndDate(startDate.Value, datePrice.DayDuration, datePrice.NightDuration, datePrice.HourDuration);
                var endTime = endDate.TimeOfDay;

                var dateRequest = new TourDateRequest()
                {
                    AvailableSpaces = Tour.MaxCapacity,
                    Id = DefaultIdType.NewGuid(),
                    PriceOverride = RequestModel.PriceOverride,
                    EndDate = endDate,
                    EndTime = endTime,
                    StartDate = startDate,
                    StartTime = startTime,
                    TourId = RequestModel.TourId,
                    TourPriceId = tourPriceId
                };

                Tour.TourDates?.Add(dateRequest);
                datePrice.Dates?.Add(dateRequest);
            }
        }
    }

    private static List<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate)
    {
        var datesInRange = new List<DateTime>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            datesInRange.Add(date);
        }

        return datesInRange;
    }

    private string PriceToString(DefaultIdType? tourPriceId)
    {
        if (Tour.TourPrices == null) return string.Empty;

        if (tourPriceId != null && tourPriceId.Value == AllPricesId)
            return "All Prices";

        if (!tourPriceId.HasValue) return string.Empty;
        
        var tourPrice = Tour.TourPrices.FirstOrDefault(x => x.Id == tourPriceId.Value);

        return tourPrice != null ? $"${tourPrice.Price} {tourPrice.Title}" : string.Empty;

    }
}