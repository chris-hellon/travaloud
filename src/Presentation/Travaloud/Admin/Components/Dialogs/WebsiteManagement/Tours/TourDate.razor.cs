using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Nextended.Core.Extensions;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Common.Utils;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class TourDate : ComponentBase
{
    [Parameter] [EditorRequired] public TourDateRequest RequestModel { get; set; } = default!;

    [Parameter] public TourViewModel Tour { get; set; } = default!;

    [Parameter] public EntityServerTableContext<TourDto, DefaultIdType, TourViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    public EditForm EditForm { get; set; } = default!;

    public List<string> RepeatDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    private bool IsCreate => Id is null;

    private readonly List<string> _repeatConditions = ["Week(s)", "Month(s)", "Year(s)"];

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    private MudChip[] SelectedRecurringDates { get; set; } = default!;

    protected Func<DefaultIdType?, string> TourPriceToStringConverter;
    
    protected override async Task OnInitializedAsync()
    {
        TourPriceToStringConverter = GeneratePriceDisplayString;
        EditContext = new EditContext(RequestModel);

        await base.OnInitializedAsync();
    }
    
    private string GeneratePriceDisplayString(DefaultIdType? tourId)
    {
        var tourPrice = Tour.TourPrices?.FirstOrDefault(u => u.Id == tourId);

        return tourPrice != null
            ? $"{tourPrice.Price} {tourPrice.Title}"
            : string.Empty;
    }

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        if (!IsCreate) return Task.CompletedTask;
                        Tour.TourDates ??= new List<TourDateRequest>();

                        RequestModel.IsCreate = true;
                        RequestModel.Id = DefaultIdType.NewGuid();

                        if (Tour.TourPrices == null) return Task.CompletedTask;
                        var datePrice = Tour.TourPrices.FirstOrDefault(x => x.Id == RequestModel.TourPriceId);
                        
                        if (datePrice != null && RequestModel.StartDate.HasValue)
                        { 
                            var startDate = RequestModel.StartDate + RequestModel.StartTime;
                            if (startDate.HasValue)
                            {
                                RequestModel.StartDate = startDate;
                                RequestModel.EndDate = DateTimeUtils.CalculateEndDate(startDate.Value, datePrice.DayDuration,
                                    datePrice.NightDuration, datePrice.HourDuration);
                                RequestModel.EndTime = RequestModel.EndDate.Value.TimeOfDay;
                            }

                            datePrice.Dates ??= new List<TourDateRequest>();
                        }

                        if (Tour.TourDates.Any())
                        {
                            var lastDate = Tour.TourDates.Last();
                            if (lastDate.StartDate == null) return Task.CompletedTask;
                            if (RequestModel.Repeats)
                            {
                                AddRecurringDates(datePrice);
                            }
                            else
                            {
                                Tour.TourDates.Add(RequestModel);
                                datePrice?.Dates?.Add(RequestModel);
                            }
                        }
                        else
                        {
                            if (RequestModel.Repeats)
                            {
                                AddRecurringDates(datePrice);
                            }
                            else
                            {
                                Tour.TourDates.Add(RequestModel);
                                datePrice?.Dates?.Add(RequestModel);
                            }
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Tour Date {(IsCreate ? L["Created"] : L["Updated"])}. Please note this is not final, you must Save the Tour to confirm these Dates."))
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


    public void ShowHelpDialog()
    {
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {
                nameof(GenericDialog.ContentText),
                "Select a Start Date and a Start Time, then select a price.<br /><br />After selecting a price, the system will calculate an End Date, based on the duration of the price.<br /><br />If you want to create a recurring date, toggle the recurring date button. This will allow you to create a date that repeats, for example, if you wish to create a date that runs every Monday for 2 months, select the Start Date as Monday, toggle the recurring date button, then select Repeats Duration equals 2, and the time period equals Month(s).<br /><br />This will then generate a Tour Date for each Monday within a 2 month period from the Start Date."
            },
            {nameof(GenericDialog.TitleText), "Guide"}
        };

        DialogService.Show<GenericDialog>(string.Empty, parameters, options);
    }

    private void AddRecurringDates(TourPriceRequest? tourPrice)
    {
        if (tourPrice == null || !RequestModel.Repeats || !RequestModel.EndDate.HasValue ||
            !RequestModel.EndTime.HasValue || !RequestModel.StartDate.HasValue ||
            !RequestModel.StartTime.HasValue) return;

        DateTime endDate;
        DateTime startDate;

        var endTime = RequestModel.EndTime.Value;
        var startTime = RequestModel.StartTime.Value;

        var dates = new List<TourDateRequest>();
        
        switch (RequestModel.RepeatsCondition)
        {
            case "Week(s)":
                for (var i = 0; i < RequestModel.RepeatsDuration; i++)
                {
                    startDate = RequestModel.StartDate.Value.AddDays(i * 7);
                    endDate = RequestModel.EndDate.Value.AddDays(i * 7);

                    if (startDate > DateTime.Now.AddYears(3))
                    {
                        Snackbar.Add($"Dates greater than {DateTime.Now.AddYears(3).ToShortDateString()} were unable to be added.", Severity.Info);
                        break;
                    }
                    
                    if (dates.Count >= 1000)
                    {
                        Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {startDate.Date.ToShortDateString()} onwards have not been added.", Severity.Info);
                        break;
                    }
                    
                    var date = new TourDateRequest()
                    {
                        AvailableSpaces = Tour.MaxCapacity,
                        PriceOverride = RequestModel.PriceOverride,
                        EndDate = endDate,
                        EndTime = endTime,
                        StartDate = startDate,
                        StartTime = startTime,
                        TourId = RequestModel.TourId,
                        TourPriceId = RequestModel.TourPriceId,
                        IsCreate = true
                    };

                    dates.Add(date);  
                }

                break;
            case "Month(s)":
                for (var i = 0; i < RequestModel.RepeatsDuration; i++)
                {
                    var daysInMonth = DateTime.DaysInMonth(RequestModel.StartDate.Value.Year, RequestModel.StartDate.Value.Month);
                    var daysToAdd = (int) Math.Round((double) daysInMonth / 4);

                    for (var j = 0; j < daysToAdd; j++)
                    {
                        startDate = RequestModel.StartDate.Value.AddDays(j * 7);
                        endDate = startDate.AddDays((RequestModel.EndDate.Value - RequestModel.StartDate.Value).TotalDays);

                        if (startDate > DateTime.Now.AddYears(3))
                        {
                            Snackbar.Add($"Dates greater than {DateTime.Now.AddYears(3).ToShortDateString()} were unable to be added.", Severity.Info);
                            break;
                        }
                        
                        if (dates.Count >= 1000)
                        {
                            Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {startDate.Date.ToShortDateString()} onwards have not been added.", Severity.Info);
                            break;
                        }
                        
                        var date = new TourDateRequest()
                        {
                            AvailableSpaces = Tour.MaxCapacity,
                            PriceOverride = RequestModel.PriceOverride,
                            EndDate = endDate,
                            EndTime = endTime,
                            StartDate = startDate,
                            StartTime = startTime,
                            TourId = RequestModel.TourId,
                            TourPriceId = RequestModel.TourPriceId,
                            IsCreate = true
                        };

                        dates.Add(date);
                    }
                }

                break;
            case "Year(s)":
                for (var i = 0; i < RequestModel.RepeatsDuration; i++)
                {
                    var newDate = RequestModel.StartDate.Value.AddYears(i);
                    var daysInYear = DateTime.IsLeapYear(newDate.Year) ? 366 : 365;
                    var weeksInYear = (int) Math.Round((double) daysInYear / 7);

                    for (var j = 0; j < weeksInYear; j++)
                    {
                        startDate = RequestModel.StartDate.Value.AddDays(i * 52 * 7).AddDays(j * 7);
                        endDate = startDate.AddDays((RequestModel.EndDate.Value - RequestModel.StartDate.Value).TotalDays);

                        if (startDate > DateTime.Now.AddYears(3))
                        {
                            Snackbar.Add($"Dates greater than {DateTime.Now.AddYears(3).ToShortDateString()} were unable to be added.", Severity.Info);
                            break;
                        }

                        if (dates.Count >= 1000)
                        {
                            Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {startDate.Date.ToShortDateString()} onwards have not been added.", Severity.Info);
                            break;
                        }
                        
                        var date = new TourDateRequest()
                        {
                            AvailableSpaces = Tour.MaxCapacity,
                            PriceOverride = RequestModel.PriceOverride,
                            EndDate = endDate,
                            EndTime = endTime,
                            StartDate = startDate,
                            StartTime = startTime,
                            TourId = RequestModel.TourId,
                            TourPriceId = RequestModel.TourPriceId,
                            IsCreate = true
                        };
                        
                        dates.Add(date);
                    }
                }

                break;
        }
        
        Tour.TourDates?.AddRange(dates);
        tourPrice.Dates?.AddRange(dates);
        
        var newDates = Tour.TourDates?.Where(x => x.IsCreate);
        if (newDates == null) return;
        
        var tourDateRequests = newDates as TourDateRequest[] ?? newDates.ToArray();

        if (tourDateRequests.Length <= 1000) return;
        
        var filteredNewDates = tourDateRequests.Take(1000);
        Tour.TourDates.RemoveRange(tourDateRequests);
        tourPrice.Dates?.RemoveRange(tourDateRequests);
                
        var itemsToAdd = filteredNewDates as TourDateRequest[] ?? filteredNewDates.ToArray();
        Tour.TourDates.AddRange(itemsToAdd);
        tourPrice.Dates?.AddRange(tourDateRequests);
                
        Snackbar.Add($"A maximum of 1000 dates can be added per request. Any dates from {itemsToAdd.Last().StartDate?.ToShortDateString()} onwards have been removed. Please submit and add more dates in the next request.", Severity.Info);
    }
}