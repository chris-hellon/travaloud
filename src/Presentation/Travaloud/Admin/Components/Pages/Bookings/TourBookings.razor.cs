using System.Drawing.Imaging;
using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using QRCoder;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.Dialogs.Bookings;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Bookings;

public partial class TourBookings
{
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IToursService ToursServive { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Inject] protected IStripeService StripeService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = default!;

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    private EntityServerTableContext<BookingDto, Guid, TourBookingViewModel> Context { get; set; } = default!;

    private EntityTable<BookingDto, Guid, TourBookingViewModel> _table = default!;
    
    private MudDateRangePicker _bookingDateRangePicker = default!;

    private MudDateRangePicker _tourDateRangePicker = default!;

    private MudTable<UpdateBookingItemRequest> _itemsTable = default!;
    
    private ICollection<TourDto> Tours { get; set; } = default!;

    private MudAutocomplete<UserDetailsDto> _guestsList = default!;
    
    private static Dictionary<string, bool> WizardSteps { get; set; } = new()
    {
        {"Select Primary Guest", true},
        {"Select Tours", true},
        {"Select Additional Guests", true}
    };
    
    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<BookingDto, Guid, TourBookingViewModel>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingDto>(booking => booking.InvoiceId, L["Reference"], "InvoiceId"),
                new EntityField<BookingDto>(booking => booking.Description, L["Description"], "Description"),
                new EntityField<BookingDto>(booking => booking.GuestName, L["Guest"], "GuestName"),
                // new EntityField<BookingDto>(booking => booking.GuestEmail, L["Guest Email"], "GuestEmail"),
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate"),
                new EntityField<BookingDto>(booking => $"$ {string.Format("{0:n2}", booking.TotalAmount)}", L["Total Amount"], "TotalAmount"),
                new EntityField<BookingDto>(booking => booking.IsPaid, L["Is Paid"], "IsPaid")
            ],
            enableAdvancedSearch: false,
            canViewEntityFunc: booking => booking.IsPaid,
            canDeleteEntityFunc: booking => !booking.IsPaid,
            canUpdateEntityFunc: booking => !booking.IsPaid,
            idFunc: booking => booking.Id,
            searchFunc: async (filter) =>
            {
                var tours = await ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999});
                Tours = tours?.Data!;

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
                var tours = await GetTours(); 
                
                var tourBooking = new TourBookingViewModel
                {
                    //Guests = guestsAndTours.Item1,
                    Tours = tours,
                    CurrencyCode = "USD"
                };

                return tourBooking;
            },
            getDetailsFunc: async (id) =>
            {
                var tourBooking = await BookingsService.GetAsync(id);
                var parsedModel = tourBooking.Adapt<TourBookingViewModel>();

                var toursTask = GetTours();
                var guestTask = UserService.GetAsync(parsedModel.GuestId, CancellationToken.None);

                await Task.WhenAll(toursTask, guestTask);
                
                //parsedModel.Guests = guestsAndTours.Item1;
                parsedModel.Tours = toursTask.Result;
                parsedModel.CurrencyCode = "USD";
                parsedModel.Id = id;
                parsedModel.Guest = guestTask.Result;
                
                return parsedModel;
            },
            createFunc: async (booking) =>
            {
                var parsedBooking = booking.Adapt<CreateBookingRequest>();
                parsedBooking.BookingDate = DateTime.Now;
                parsedBooking.GuestId = booking.Guest.Id.ToString();
                parsedBooking.GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
                parsedBooking.GuestEmail = booking.Guest.Email;
                parsedBooking.TotalAmount = booking.Items.Sum(item => item.Amount.Value *
                                                                       (item.Guests != null && item.Guests.Any() ? item.Guests.Count + 1 : 1));
                
                var bookingId = await BookingsService.CreateAsync(parsedBooking);

                if (bookingId.HasValue)
                {
                    await GenerateBookingQRCode(bookingId.Value, booking.Guest.Email);
                }
            },
            updateFunc: async (id, booking) =>
            {
                var request = booking.Adapt<UpdateBookingRequest>();
                request.GuestId = booking.Guest.Id.ToString();
                request.GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
                request.GuestEmail = booking.Guest.Email;
                request.TotalAmount = booking.Items.Sum(item => item.Amount.Value *
                                                                 (item.Guests != null && item.Guests.Any() ? item.Guests.Count + 1: 1)) ;
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
                    exportFilter.BookingEndDate = exportFilter.BookingEndDate.Value + new TimeSpan(0, 23, 59, 59, 999);
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

    private async Task<ICollection<TourDto>> GetTours()
    {
        var tours = await ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999});
        return tours?.Data ?? [];
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

    private async Task InvokeCreateGuestDialog(TourBookingViewModel tourBooking)
    {
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(CreateGuest.TitleLabel), "Primary"},
            {nameof(CreateGuest.EmailRequired), true}
        };

        var dialog = await DialogService.ShowAsync<CreateGuest>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var guests = await UserService.GetListAsync(TravaloudRoles.Guest);
            //tourBooking.Guests = guests;

            var guestId = result.Data.ToString();
            tourBooking.GuestId = guestId;
            tourBooking.Guest = guests.FirstOrDefault(x => x.Id == guestId);
        }
        
        Context.AddEditModal?.ForceRender();
    }
    
    private async Task GenerateBookingQRCode(Guid bookingId, string guestEmail)
    {
        await LoadingService.ToggleLoaderVisibility(true);
        
        var stripeCheckoutUrl = await StripeService.CreateStripeQrCodeUrl(new CreateStripeQrCodeRequest()
        {
            BookingId = bookingId,
            GuestEmail = guestEmail
        });
                    
        using MemoryStream ms = new();
        QRCodeGenerator qrCodeGenerate = new();
        var qrCodeData = qrCodeGenerate.CreateQrCode(stripeCheckoutUrl, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new(qrCodeData);
        using var qrBitMap = qrCode.GetGraphic(20);
        qrBitMap.Save(ms, ImageFormat.Png);
                        
        var base64 = Convert.ToBase64String(ms.ToArray());
        var qrCodeImageUrl = $"data:image/png;base64,{base64}";

        await LoadingService.ToggleLoaderVisibility(false);
        await InvokeStripeQrCodeUrl(qrCodeImageUrl);
    }

    private async Task InvokeStripeQrCodeUrl(string qrcodeImageUrl)
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
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is not final. To confirm deltion, save the booking."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Item", id)}
        };

        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var item = tourBooking.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // await BookingsService.DeleteItemAsync(item.Id);

                tourBooking.Items.Remove(item);
                tourBooking.ItemQuantity++;

                if (item.Amount.HasValue)
                {
                    tourBooking.TotalAmount -= item.Amount.Value *
                                               (item.Guests != null && item.Guests.Any() ? item.Guests.Count : 1);
                }
            }

            Context.AddEditModal?.ForceRender();
        }
    }
    
    private async Task<IEnumerable<UserDetailsDto>> SearchGuests(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Array.Empty<UserDetailsDto>();

        var guests = await UserService.SearchByDapperAsync(new SearchByDapperRequest()
        {
            Keyword = value,
            Role = "Guest",
            PageNumber = 1,
            PageSize = 99999,
            TenantId = TenantInfo.Id
        }, token);

        if (guests.Data.Count != 0) return guests.Data;
        
        return Array.Empty<UserDetailsDto>();
    }

    private string GetUserDetailsLabel(UserDetailsDto e)
    {
        return
            $"{e.FirstName} {e.LastName}{(e.DateOfBirth.HasValue ? $" - {e.DateOfBirth?.ToShortDateString()}" : "")}{(!string.IsNullOrEmpty(e.Gender) ? $" - {e.Gender.GenderMatch()}" : "")}{(!string.IsNullOrEmpty(e.Nationality) ? $" - {e.Nationality?.TwoLetterCodeToCountry()} - " : "")}{e.Email}";
    }
    
    private static void ShowBookingItemRooms(BookingItemDetailsDto request, BookingDto bookingItem)
    {
        request.ShowDetails = !request.ShowDetails;
        if (bookingItem.Items == null) return;
        foreach (var otherTrail in bookingItem.Items.Where(x => x.Id != request.Id))
        {
            otherTrail.ShowDetails = false;
        }
    }
}

public class TourBookingViewModel : UpdateBookingRequest
{
    //public ICollection<UserDetailsDto>? Guests { get; set; }
    public ICollection<TourDto>? Tours { get; set; }
    public UserDetailsDto? Guest { get; set; }
}