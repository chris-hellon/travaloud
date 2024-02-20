using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.Bookings;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.WebsiteManagement.Tours;

public partial class TourBookingItem : ComponentBase
{
    [Inject] protected IToursService ToursService { get; set; } = default!;

    [Parameter] [EditorRequired] public UpdateBookingItemRequest RequestModel { get; set; } = default!;

    [Parameter] public TourBookingViewModel TourBooking { get; set; } = default!;

    [Parameter]
    public EntityServerTableContext<BookingDto, Guid, TourBookingViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    [Parameter] public ICollection<TourDto> Tours { get; set; } = default!;

    private ICollection<TourDateDto> TourDates { get; set; } = default!;

    public EditForm EditForm { get; set; } = default!;

    private bool IsCreate => Id is null;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(RequestModel);

        await base.OnInitializedAsync();
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

                        if (TourBooking.Items.Count > 0)
                        {
                            var lastItem = TourBooking.Items.Last();
                            if (lastItem.TourId == null) return Task.CompletedTask;

                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value;
                            }
                        }
                        else
                        {
                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value;
                            }
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Booking Tour {(IsCreate ? L["Created"] : L["Updated"])}."))
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

    private async Task OnTourValueChanged(Guid? tourId)
    {
        if (tourId.HasValue)
        {
            var tourDates = await ToursService.GetTourDatesAsync(tourId.Value, 1);

            TourDates = tourDates.Data;

            if (tourDates.Data.Count == 0)
            {
                RequestModel.TourDateId = null;
            }
            else
            {
                bool? any = TourBooking.Items.Count != 0;

                if (any == true)
                {
                    var tourDateIds = TourBooking.Items.Select(x => x.TourDateId);

                    TourDates = TourDates.Where(x => !tourDateIds.Contains(x.Id)).ToList();
                }
            }

            RequestModel.TourId = tourId.Value;
            StateHasChanged();
        }
    }

    private Task OnTourDateValueChanged(Guid? tourDateId)
    {
        if (!tourDateId.HasValue) return Task.CompletedTask;
        var tourDate = TourDates.FirstOrDefault(x => x.Id == tourDateId.Value);

        if (tourDate != null)
        {
            RequestModel.StartDate = tourDate.StartDate;
            RequestModel.EndDate = tourDate.EndDate;
            RequestModel.Amount = tourDate.TourPrice?.Price;
            RequestModel.TourDateId = tourDateId.Value;
            RequestModel.TourDate = tourDate.Adapt<UpdateTourDateRequest>();
        }

        StateHasChanged();

        return Task.CompletedTask;
    }
}