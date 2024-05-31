using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Nextended.Core.Extensions;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.WebsiteManagement.Tours;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class DeleteTourDates : ComponentBase
{
    [Parameter] public required TourViewModel Tour { get; set; }

    [Parameter] public required IList<TimeSpan> CurrentTimes { get; set; }

    [Parameter] public EntityServerTableContext<TourDto, DefaultIdType, TourViewModel> Context { get; set; } = default!;

    [Inject] private IBookingsService BookingsService { get; set; }

    public DateRange? DeleteDateRange { get; set; }

    public TimeSpan? DeleteTimeSpan { get; set; }

    public EditForm EditForm { get; set; } = default!;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(this);

        await base.OnInitializedAsync();
    }

    private EditContext? EditContext { get; set; }

    private void Cancel() =>
        MudDialog.Cancel();

    private async Task RemoveDatesWithTimeSpan(ICollection<DateTime> dateRange)
    {
        var datesToRemove = Tour.TourDates.Where(x =>
            x.StartDate.HasValue &&
            dateRange.Contains(x.StartDate.Value.Date + DeleteTimeSpan.Value));

        var tourDateRequests = datesToRemove as TourDateRequest[] ?? datesToRemove.ToArray();
        var datesToRemovePrices = tourDateRequests
            .Where(x => x.TourPriceId.HasValue)
            .Select(x => x.TourPriceId.Value).Distinct();

        var toursDates = tourDateRequests.Select(x => x.Id);
        var tourDateIds = toursDates as DefaultIdType[] ?? toursDates.ToArray();

        var bookingsExists =
            await BookingsService.GetBookingsByDatesAsync(new BookingsByTourStartDatesRequest(tourDateIds));

        if (bookingsExists.Any())
        {
            tourDateRequests = tourDateRequests.Where(x => !bookingsExists.Contains(x.Id)).ToArray();
        }

        Tour.TourDates.RemoveRange(tourDateRequests);

        var tourPrices = Tour.TourPrices?.Where(x => datesToRemovePrices.Contains(x.Id));

        foreach (var tourPrice in tourPrices)
        {
            var tourPriceDatesToRemove = tourDateRequests.Where(x => x.TourPriceId == tourPrice.Id);
            tourPrice.Dates?.RemoveRange(tourPriceDatesToRemove);
        }
    }

    private async Task RemoveDatesWithDateRangeAndNoTimeSpan(ICollection<DateTime> dateRange)
    {
        var datesToRemove = Tour.TourDates.Where(x =>
            x.StartDate.HasValue &&
            dateRange.Contains(x.StartDate.Value.Date));

        var tourDateRequests = datesToRemove as TourDateRequest[] ?? datesToRemove.ToArray();
        var datesToRemovePrices = tourDateRequests
            .Where(x => x.TourPriceId.HasValue)
            .Select(x => x.TourPriceId.Value).Distinct();

        var toursDates = tourDateRequests.Select(x => x.Id);
        var tourDateIds = toursDates as DefaultIdType[] ?? toursDates.ToArray();

        var bookingsExists =
            await BookingsService.GetBookingsByDatesAsync(new BookingsByTourStartDatesRequest(tourDateIds));

        if (bookingsExists.Any())
        {
            tourDateRequests = tourDateRequests.Where(x => !bookingsExists.Contains(x.Id)).ToArray();
        }

        Tour.TourDates.RemoveRange(tourDateRequests);

        var tourPrices = Tour.TourPrices?.Where(x => datesToRemovePrices.Contains(x.Id));

        foreach (var tourPrice in tourPrices)
        {
            var tourPriceDatesToRemove = tourDateRequests.Where(x => x.TourPriceId == tourPrice.Id);
            tourPrice.Dates?.RemoveRange(tourPriceDatesToRemove);
        }
    }

    private async Task RemoveDatesWithTimeSpan()
    {
        var datesToRemove = Tour.TourDates?.Where(x =>
            x.StartDate.HasValue && x.StartDate.Value.TimeOfDay == DeleteTimeSpan!.Value);

        var tourDateRequests = datesToRemove as TourDateRequest[] ?? datesToRemove.ToArray();
        var datesToRemovePrices = tourDateRequests
            .Where(x => x.TourPriceId.HasValue)
            .Select(x => x.TourPriceId.Value).Distinct();

        var toursDates = tourDateRequests.Select(x => x.Id);
        var tourDateIds = toursDates as DefaultIdType[] ?? toursDates.ToArray();

        var bookingsExists =
            await BookingsService.GetBookingsByDatesAsync(new BookingsByTourStartDatesRequest(tourDateIds));

        if (bookingsExists.Any())
        {
            tourDateRequests = tourDateRequests.Where(x => !bookingsExists.Contains(x.Id)).ToArray();
        }

        Tour.TourDates.RemoveRange(tourDateRequests);

        var tourPrices = Tour.TourPrices?.Where(x => datesToRemovePrices.Contains(x.Id));

        foreach (var tourPrice in tourPrices)
        {
            var tourPriceDatesToRemove = tourDateRequests.Where(x => x.TourPriceId == tourPrice.Id);
            tourPrice.Dates?.RemoveRange(tourPriceDatesToRemove);
        }
    }

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (await ServiceHelper.ExecuteCallGuardedAsync(async () =>
                {
                    if (DeleteDateRange != null)
                    {
                        var dateRange = GetDatesInRange(DeleteDateRange.Start.Value, DeleteDateRange.End.Value, DeleteTimeSpan ?? null);

                        if (DeleteTimeSpan.HasValue)
                        {
                            await RemoveDatesWithTimeSpan(dateRange);
                        }
                        else
                        {
                            await RemoveDatesWithDateRangeAndNoTimeSpan(dateRange);
                        }
                    }
                    else if (DeleteTimeSpan.HasValue)
                    {
                        await RemoveDatesWithTimeSpan();
                    }
                    else
                    {
                        var toursDates = Tour.TourDates.Select(x => x.Id);
                        var tourDateIds = toursDates as DefaultIdType[] ?? toursDates.ToArray();

                        var bookingsExists =
                            await BookingsService.GetBookingsByDatesAsync(
                                new BookingsByTourStartDatesRequest(tourDateIds));

                        if (bookingsExists.Any())
                        {
                            var matchedTourDates = Tour.TourDates.Where(x => bookingsExists.Contains(x.Id));
                            Tour.TourDates = matchedTourDates.ToList();
                        }
                        else
                        {
                            Tour.TourDates = new List<TourDateRequest>();
                        }
                    }
                },
                Snackbar,
                Logger,
                $"Tour Dates deleted. Please note this is not final, you must Save the Tour to confirm these Dates."))
        {
            MudDialog.Close(this);
        }

        await LoadingService.ToggleLoaderVisibility(false);
    }

    private static List<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate, TimeSpan? timeSpan)
    {
        var datesInRange = new List<DateTime>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (timeSpan.HasValue)
                datesInRange.Add(date.Date + timeSpan.Value);
            else 
                datesInRange.Add(date);
        }

        return datesInRange;
    }
}