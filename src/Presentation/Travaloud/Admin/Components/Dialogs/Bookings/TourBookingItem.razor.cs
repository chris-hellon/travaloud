using System.Text.Json;
using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Org.BouncyCastle.Ocsp;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.Bookings;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class TourBookingItem : ComponentBase
{
    [Inject] protected IToursService ToursService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] [EditorRequired] public UpdateBookingItemRequest RequestModel { get; set; } = default!;

    [Parameter] public TourBookingViewModel TourBooking { get; set; } = default!;

    [Parameter] public EntityServerTableContext<BookingDto, Guid, TourBookingViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }

    [Parameter] public ICollection<TourDto> Tours { get; set; } = default!;

    public ICollection<TourDateDto> TourDates { get; set; } = default!;
    
    public IEnumerable<TourPickupLocationDto> TourPickupLocations { get; set; } = default!;

    public EditForm EditForm { get; set; } = default!;

    public bool IsCreate => Id is null;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }

    protected Func<Guid?, string> TourDateToStringConverter;
    protected Func<Guid?, string> TourToStringConverter;

    protected override async Task OnInitializedAsync()
    {
        TourDateToStringConverter = GenerateTourDateDisplayString;
        TourToStringConverter = GenerateTourDisplayString;

        EditContext = new EditContext(RequestModel);

        if (RequestModel.TourId.HasValue)
        {
            if (RequestModel.Guests != null && RequestModel.Guests.Any())
            {
                var userIds = RequestModel.Guests.Select(x => x.GuestId).ToList();
                var guests = await UserService.SearchAsync(userIds, CancellationToken.None);
                
                foreach (var guest in RequestModel.Guests)
                {
                    var matchedGuest = guests.FirstOrDefault(x => x.Id == Guid.Parse(guest.GuestId));

                    if (matchedGuest == null) continue;
                    guest.FirstName = matchedGuest.FirstName;
                    guest.LastName = matchedGuest.LastName;
                    guest.EmailAddress = matchedGuest.Email;
                }
            }
            
            var tourDatesRequest = Task.Run(() => ToursService.GetTourDatesAsync(RequestModel.TourId.Value, 1));
            var tourPickupLocationsRequest = Task.Run(() => ToursService.GetTourPickupLocations(RequestModel.TourId.Value));

            await Task.WhenAll(tourDatesRequest, tourPickupLocationsRequest);

            var tourDates = tourDatesRequest.Result;
            
            TourDates = tourDates.Data;
            TourPickupLocations = tourPickupLocationsRequest.Result;
        }

        await base.OnInitializedAsync();
    }

    private string GenerateTourDateDisplayString(Guid? tourDateId)
    {
        var tourDate = TourDates != null ? TourDates.FirstOrDefault(u => u.Id == tourDateId) : null;

        return tourDate != null
            ? $"{tourDate.StartDate} ({tourDate.TourPrice?.Price}) - {tourDate.AvailableSpaces} spaces available"
            : string.Empty;
    }

    private string GenerateTourDisplayString(Guid? tourId)
    {
        var tour = Tours.FirstOrDefault(u => u.Id == tourId);

        return tour?.Name ?? string.Empty;
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
                        TourBooking.Items ??= new List<UpdateBookingItemRequest>();

                        if (TourBooking.Items.Count > 0)
                        {
                            var lastItem = TourBooking.Items.Last();
                            if (lastItem?.TourId == null) return Task.CompletedTask;

                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value *
                                                           (RequestModel.Guests != null && RequestModel.Guests.Any()
                                                               ? RequestModel.Guests.Count
                                                               : 1);
                            }
                        }
                        else
                        {
                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value *
                                                           (RequestModel.Guests != null && RequestModel.Guests.Any()
                                                               ? RequestModel.Guests.Count
                                                               : 1);
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
            var tourDatesRequest = Task.Run(() => ToursService.GetTourDatesAsync(tourId.Value, 1));
            var tourPickupLocationsRequest = Task.Run(() => ToursService.GetTourPickupLocations(tourId.Value));

            await Task.WhenAll(tourDatesRequest, tourPickupLocationsRequest);

            var tourDates = tourDatesRequest.Result;

            TourPickupLocations = tourPickupLocationsRequest.Result;
            TourDates = tourDates.Data.Where(x => x.StartDate > DateTime.Now).ToList();

            RequestModel.TourDateId = null;
            RequestModel.PickupLocation = null;
            
            if (tourDates.Data.Count == 0)
            {
                RequestModel.TourDateId = null;
            }
            else if (TourBooking.Items?.Any() == true)
            {
                var tourDateIds = TourBooking.Items.Select(x => x.TourDateId);

                TourDates = TourDates.Where(x => !tourDateIds.Contains(x.Id)).ToList();
            }

            RequestModel.TourId = tourId.Value;
            StateHasChanged();
        }
    }

    private void OnTourDateValueChanged(Guid? tourDateId)
    {
        if (!tourDateId.HasValue) return;

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
    }

    public async Task InvokeBookingItemGuestDialog(UpdateBookingItemRequest bookingItem)
    {
        var initialModel =
            JsonSerializer.Deserialize<IList<BookingItemGuestRequest>>(JsonSerializer.Serialize(bookingItem.Guests)) ??
            new List<BookingItemGuestRequest>();
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(TourBookingItemGuest.RequestModel), new BookingItemGuestRequest() {BookingItemId = bookingItem.Id}},
            {nameof(TourBookingItemGuest.TourBookingItem), bookingItem}
        };

        var dialog = await DialogService.ShowAsync<TourBookingItemGuest>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            bookingItem.Guests = initialModel;
        }


        StateHasChanged();
        Context.AddEditModal?.ForceRender();
    }


    public async Task RemoveGuestRow(UpdateBookingItemRequest bookingItem, string id)
    {
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is final."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Price", id)}
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var item = bookingItem.Guests?.FirstOrDefault(x => x.GuestId == id);

            if (item != null)
            {
                //await BookingsService.DeleteItemAsync(item.Id);

                bookingItem.Guests?.Remove(item);
            }

            Context.AddEditModal?.ForceRender();
        }
    }
}