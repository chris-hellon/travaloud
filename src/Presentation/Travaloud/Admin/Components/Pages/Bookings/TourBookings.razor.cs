using System.Drawing.Imaging;
using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using QRCoder;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Bookings;

public partial class TourBookings
{
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IToursService ToursServive { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Inject] protected IStripeService StripeService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = default!;

    private EntityServerTableContext<BookingDto, Guid, TourBookingViewModel> Context { get; set; } = default!;

    private EditContext? _editContext { get; set; }

    private EntityTable<BookingDto, Guid, TourBookingViewModel> _table = default!;

    private List<string> _wizardSteps => new List<string>()
        {"Basic Information", "Description", "Additional Information", "Image"};

    private MudDateRangePicker _bookingDateRangePicker = default!;

    private MudDateRangePicker _tourDateRangePicker = default!;

    private MudTable<UpdateBookingItemRequest>? _itemsTable;

    private ICollection<TourDto> _tours { get; set; } = default!;
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<BookingDto, Guid, TourBookingViewModel>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingDto>(booking => booking.InvoiceId, L["Reference"], "Reference"),
                new EntityField<BookingDto>(booking => booking.Description, L["Description"], "Description"),
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate"),
                new EntityField<BookingDto>(booking => $"{booking.TotalAmount} {booking.CurrencyCode}", L["Total Amount"], "TotalAmount"),
                new EntityField<BookingDto>(booking => booking.IsPaid, L["Is Paid"], "IsPaid")
            ],
            enableAdvancedSearch: false,
            idFunc: booking => booking.Id,
            searchFunc: async (filter) =>
            {
                var tours = await ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999});
                _tours = tours?.Data!;

                var adaptedFilter = filter.Adapt<SearchBookingsRequest>();
                adaptedFilter.IsTours = true;
                adaptedFilter.TourId = SearchTourId;
                adaptedFilter.BookingStartDate = SearchBookingDateRange?.Start;
                adaptedFilter.BookingEndDate = SearchBookingDateRange?.End;
                adaptedFilter.TourStartDate = SearchTourStartDateRange?.Start;
                adaptedFilter.TourEndDate = SearchTourStartDateRange?.End;

                if (adaptedFilter.BookingEndDate.HasValue)
                {
                    adaptedFilter.BookingEndDate =
                        adaptedFilter.BookingEndDate.Value + new TimeSpan(0, 11, 59, 59, 999);
                }

                if (adaptedFilter.BookingStartDate.HasValue)
                {
                    adaptedFilter.BookingStartDate =
                        adaptedFilter.BookingStartDate.Value + new TimeSpan(0, 00, 00, 00, 00);
                }

                if (adaptedFilter is {TourEndDate: not null, TourStartDate: not null})
                {
                    adaptedFilter.TourEndDate = adaptedFilter.TourEndDate.Value + new TimeSpan(0, 23, 59, 59, 999);
                    adaptedFilter.TourStartDate = adaptedFilter.TourStartDate.Value + new TimeSpan(0, 00, 00, 00, 00);
                }

                var request = await BookingsService.SearchAsync(adaptedFilter);

                return request.Adapt<PaginationResponse<BookingDto>>();
            },
            hasExtraActionsFunc: () => true,
            getDefaultsFunc: async () =>
            {
                var guestsAndTours = await GetGuestsAndTours(); 
                
                var tourBooking = new TourBookingViewModel
                {
                    Guests = guestsAndTours.Item1,
                    Tours = guestsAndTours.Item2,
                    CurrencyCode = "USD"
                };

                return tourBooking;
            },
            getDetailsFunc: async (id) =>
            {
                var tourBooking = await BookingsService.GetAsync(id);
                var parsedModel = tourBooking.Adapt<TourBookingViewModel>();
                var guestsAndTours = await GetGuestsAndTours(); 
                
                parsedModel.Guests = guestsAndTours.Item1;
                parsedModel.Tours = guestsAndTours.Item2;
                parsedModel.CurrencyCode = "USD";
                parsedModel.Id = id;
                parsedModel.Guest = guestsAndTours.Item1.FirstOrDefault(x => x.Id.ToString() == parsedModel.GuestId);
                
                return parsedModel;
            },
            createFunc: async (booking) =>
            {
                var parsedBooking = booking.Adapt<CreateBookingRequest>();
                parsedBooking.BookingDate = DateTime.Now;
                parsedBooking.GuestId = booking.Guest.Id.ToString();
                
                var bookingId = await BookingsService.CreateAsync(parsedBooking);

                if (bookingId.HasValue)
                {
                    var stripeCheckoutUrl = await StripeService.CreateStripeQrCodeUrl(new CreateStripeQrCodeRequest()
                    {
                        BookingId = bookingId.Value,
                        GuestEmail = booking.Guest.Email
                    });
                    
                    using MemoryStream ms = new();
                    QRCodeGenerator qrCodeGenerate = new();
                    var qrCodeData = qrCodeGenerate.CreateQrCode(stripeCheckoutUrl, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new(qrCodeData);
                    using var qrBitMap = qrCode.GetGraphic(20);
                    qrBitMap.Save(ms, ImageFormat.Png);
                        
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    var qrCodeImageUrl = $"data:image/png;base64,{base64}";

                    await InvokeStripeQrCodeUrl(qrCodeImageUrl);
                }
            },
            updateFunc: async (id, booking) =>
            {
                var request = booking.Adapt<UpdateBookingRequest>();
                request.GuestId = booking.Guest.Id.ToString();
                
                await BookingsService.UpdateAsync(id, request);
            },
            deleteFunc: async id => await BookingsService.DeleteAsync(id),
            exportFunc: async filter =>
            {
                var exportFilter = filter.Adapt<ExportBookingsRequest>();

                exportFilter.Description = SearchDescription == default ? null : SearchDescription;
                exportFilter.BookingStartDate = SearchBookingDateRange?.Start;
                exportFilter.BookingEndDate = SearchBookingDateRange?.End;
                exportFilter.TourStartDate = SearchTourStartDateRange?.Start;
                exportFilter.TourEndDate = SearchTourStartDateRange?.End;
                exportFilter.IsTourBookings = true;
                
                if (exportFilter.BookingEndDate.HasValue)
                {
                    exportFilter.BookingEndDate = exportFilter.BookingEndDate.Value + new TimeSpan(0, 11, 59, 59, 999);
                }

                if (exportFilter.BookingStartDate.HasValue)
                {
                    exportFilter.BookingStartDate =
                        exportFilter.BookingStartDate.Value + new TimeSpan(0, 00, 00, 00, 00);
                }

                if (exportFilter.TourEndDate.HasValue)
                {
                    exportFilter.TourEndDate = exportFilter.TourEndDate.Value + new TimeSpan(0, 23, 59, 59, 999);
                }

                if (exportFilter.TourStartDate.HasValue)
                {
                    exportFilter.TourStartDate = exportFilter.TourStartDate.Value + new TimeSpan(0, 00, 00, 00, 00);
                }

                exportFilter.TourId = SearchTourId;

                return await BookingsService.ExportAsync(exportFilter);
            }
        );
    }
    
    private async Task<Tuple<ICollection<UserDetailsDto>, ICollection<TourDto>>> GetGuestsAndTours()
    {
        var guestsTask = UserService.GetListAsync(TravaloudRoles.Guest);
        var toursTask = ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999});

        await Task.WhenAll(guestsTask, toursTask);

        return new Tuple<ICollection<UserDetailsDto>, ICollection<TourDto>>(guestsTask.Result, toursTask.Result?.Data ?? []);
    }

    private string? _searchDescription;

    private string SearchDescription
    {
        get => _searchDescription ?? string.Empty;
        set
        {
            _searchDescription = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private DateTime? _searchBookingDate;

    private DateTime? SearchBookingDate
    {
        get => _searchBookingDate;
        set
        {
            _searchBookingDate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private Guid? _searchTourId;

    private Guid? SearchTourId
    {
        get => _searchTourId;
        set
        {
            _searchTourId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private DateRange? _searchBookingDateRange;

    private DateRange? SearchBookingDateRange
    {
        get => _searchBookingDateRange;
        set
        {
            _searchBookingDateRange = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private DateRange? _searchTourStartDateRange;

    private DateRange? SearchTourStartDateRange
    {
        get => _searchTourStartDateRange;
        set
        {
            _searchTourStartDateRange = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private void ShowBtnPress(BookingDto request)
    {
        request.ShowDetails = !request.ShowDetails;
        if (Context.EntityList == null) return;
        foreach (var otherTrail in Context.EntityList.Where(x => x.Id != request.Id))
        {
            otherTrail.ShowDetails = false;
        }
    }

    public async Task InvokeBookingItemDialog(UpdateBookingItemRequest requestModel, TourBookingViewModel tourBooking, bool isCreate = false)
    {
        var initialModel = JsonSerializer.Deserialize<IList<UpdateBookingItemRequest>>(JsonSerializer.Serialize(tourBooking.Items)) ?? new List<UpdateBookingItemRequest>();
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.Bookings.TourBookingItem.RequestModel), requestModel},
            {nameof(Dialogs.Bookings.TourBookingItem.TourBooking), tourBooking},
            {nameof(Dialogs.Bookings.TourBookingItem.Context), Context},
            {nameof(Dialogs.Bookings.TourBookingItem.Guests), tourBooking.Guests},
            {nameof(Dialogs.Bookings.TourBookingItem.Id), isCreate ? null : requestModel.Id},
            {nameof(Dialogs.Bookings.TourBookingItem.Tours), tourBooking.Tours},
        };

        var dialog = await DialogService.ShowAsync<Components.Dialogs.Bookings.TourBookingItem>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            tourBooking.Items = initialModel;
        }

        Context.AddEditModal?.ForceRender();
    }

    public async Task InvokeStripeQrCodeUrl(string qrcodeImageUrl)
    {
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(Dialogs.Bookings.StripeQrCode.QrCodeImageUrl), qrcodeImageUrl},
        };
        
        var dialog = await DialogService.ShowAsync<Components.Dialogs.Bookings.StripeQrCode>(string.Empty, parameters, options);

        var result = await dialog.Result;
    }
    
    public async Task RemoveItemRow(TourBookingViewModel tourBooking, Guid id)
    {
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is final."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Price", id)}
        };

        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var item = tourBooking.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                await BookingsService.DeleteItemAsync(item.Id);

                tourBooking.Items.Remove(item);
                tourBooking.ItemQuantity++;

                if (item.Amount.HasValue)
                {
                    tourBooking.TotalAmount -= item.Amount.Value;
                }
            }

            Context.AddEditModal?.ForceRender();
        }
    }
    
    private async Task<IEnumerable<UserDetailsDto>> SearchGuests(string value)
    {
        await Task.Delay(5);

        return (string.IsNullOrEmpty(value)
                   ? Context.AddEditModal.RequestModel.Guests
                   : Context.AddEditModal.RequestModel.Guests.Where(x => (x.FirstName.Contains(value,
                                                                              StringComparison
                                                                                  .InvariantCultureIgnoreCase) &&
                                                                          x.LastName.Contains(value,
                                                                              StringComparison
                                                                                  .InvariantCultureIgnoreCase)) ||
                                                                         x.FirstName.Contains(value,
                                                                             StringComparison
                                                                                 .InvariantCultureIgnoreCase) ||
                                                                         x.LastName.Contains(value,
                                                                             StringComparison
                                                                                 .InvariantCultureIgnoreCase))) ??
               Array.Empty<UserDetailsDto>();
    }
}

public class TourBookingViewModel : UpdateBookingRequest
{
    public ICollection<UserDetailsDto>? Guests { get; set; }
    public ICollection<TourDto>? Tours { get; set; }
    public UserDetailsDto? Guest { get; set; }
}