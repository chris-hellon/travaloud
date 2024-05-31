using System.Drawing.Imaging;
using BlazorTemplater;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using QRCoder;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.Dialogs.Bookings;
using Travaloud.Admin.Components.Dialogs.Guests;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.Bookings.Common;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Exceptions;
using Travaloud.Application.Common.Exporters;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.Common.Models;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Infrastructure.Auth;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Bookings;

public partial class TourBookings
{
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IPropertiesService PropertiesService { get; set; } = default!;

    [Inject] protected IToursService ToursServive { get; set; } = default!;

    [Inject] protected ITourCategoriesService TourCategoriesService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Inject] protected IStripeService StripeService { get; set; } = default!;

    [Inject] protected IEmailTemplateService EmailTemplateService { get; set; } = default!;

    [Inject] protected IMailService MailService { get; set; } = default!;

    [Inject] protected IAuthorizationService AuthService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = default!;

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }

    [CascadingParameter] private MudTheme? CurrentTheme { get; set; }

    private EntityServerTableContext<BookingDto, DefaultIdType, TourBookingViewModel> Context { get; set; } = default!;

    private EntityTable<BookingDto, DefaultIdType, TourBookingViewModel> _table = default!;

    private MudDateRangePicker _bookingDateRangePicker = default!;

    private MudDateRangePicker _tourDateRangePicker = default!;

    private ICollection<TourDto> Tours { get; set; } = default!;

    private bool CanRefund { get; set; }
    
    private decimal? RefundAmount { get; set; }

    private MudMessageBox AdditionalChargeMessageBox { get; set; }
    
    private RenderFragment<BookingDto> guestNameTemplate = booking => __builder =>
    {
        __builder.OpenComponent(0, typeof(GuestEditButtonComponent));
        __builder.AddAttribute(1, "GuestName", booking.GuestName);
        __builder.AddAttribute(2, "Id", booking.GuestId);
        __builder.CloseComponent();
    };

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var user = authState.User;

        CanRefund = await AuthService.HasPermissionAsync(user, TravaloudAction.Delete, TravaloudResource.TourBookings);
    }

    protected override void OnInitialized()
    {
        Context = new EntityServerTableContext<BookingDto, DefaultIdType, TourBookingViewModel>(
            entityName: L["Booking"],
            entityNamePlural: L["Bookings"],
            entityResource: TravaloudResource.Bookings,
            fields:
            [
                new EntityField<BookingDto>(booking => booking.InvoiceId, L["Reference"], "InvoiceId"),
                new EntityField<BookingDto>(booking => booking.Description, L["Description"], "Description"),
                new EntityField<BookingDto>(booking => booking.GuestName, L["Guest"], "GuestName"),
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate"),
                new EntityField<BookingDto>(booking => $"$ {booking.TotalAmount:n2}", L["Total Amount"], "TotalAmount"),
                new EntityField<BookingDto>(
                    booking => booking.IsPaid ? "Paid" :
                        booking.Refunded.HasValue && booking.Refunded.Value ? "Refunded" : booking.AmountOutstanding.HasValue ? "Partially Paid" : "Unpaid", L["Status"],
                    "IsPaid",
                    Color: booking => !booking.IsPaid && (!booking.Refunded.HasValue || !booking.Refunded.Value)
                        ? CurrentTheme.Palette.Error
                        : null)
            ],
            enableAdvancedSearch: false,
            canViewEntityFunc: booking => booking.IsPaid || (booking.Refunded.HasValue && booking.Refunded.Value),
            canDeleteEntityFunc: booking => !booking.IsPaid,
            // canUpdateEntityFunc: booking => !booking.IsPaid,
            idFunc: booking => booking.Id,
            searchFunc: async filter => await SearchTourBookings(filter),
            hasExtraActionsFunc: () => true,
            getDefaultsFunc: async () =>
            {
                var toursAndProperties = await GetToursAndProperties();

                var tourBooking = new TourBookingViewModel
                {
                    Tours = toursAndProperties.Item1,
                    Properties = toursAndProperties.Item2,
                    Categories = toursAndProperties.Item3,
                    CurrencyCode = "USD"
                };

                return tourBooking;
            },
            getDetailsFunc: async id => await GetTourBooking(id),
            createFunc: async booking => await CreateTourBooking(booking),
            updateFunc: async (id, booking) => await UpdateTourBooking(id, booking),
            deleteFunc: async id => await BookingsService.DeleteAsync(id),
            exportFunc: async filter => await ExportTourBooking(filter)
        );
    }

    private async Task<EntityTable.PaginationResponse<BookingDto>> SearchTourBookings(PaginationFilter filter)
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
                adaptedFilter.BookingEndDate.Value.Date + new TimeSpan(0, 23, 59, 59, 999);
        }

        if (adaptedFilter.BookingStartDate.HasValue)
        {
            adaptedFilter.BookingStartDate =
                adaptedFilter.BookingStartDate.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
        }

        if (adaptedFilter is {TourEndDate: not null, TourStartDate: not null})
        {
            adaptedFilter.TourEndDate = adaptedFilter.TourEndDate.Value.Date + new TimeSpan(0, 23, 59, 59, 999);
            adaptedFilter.TourStartDate = adaptedFilter.TourStartDate.Value.Date + new TimeSpan(0, 00, 00, 00, 00);
        }

        var request = await BookingsService.SearchAsync(adaptedFilter);

        var staffIds = request.Data.Select(x => x.CreatedBy.ToString()).ToList();
        var primaryGuestIds = request.Data.Where(x => !x.GuestId.IsNullOrEmpty()).Select(x => x.GuestId).ToList();
        var additionalGuestIds = request.Data
            .Where(x => x.Items != null && x.Items.Any(i => i.Guests is {Count: > 0}))
            .SelectMany(x => x.Items.Where(i => i.Guests.Count > 0)
                .SelectMany(i => i.Guests.Select(g => g.GuestId.ToString()))).ToList();

        var allUserIds = staffIds.Concat(primaryGuestIds).Concat(additionalGuestIds).ToList();

        var users = await UserService.SearchAsync(allUserIds, CancellationToken.None);

        if (users.Count == 0) return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
        {
            var bookings = request.Data.Select(x =>
            {
                var staffMember = users.FirstOrDefault(s => s.Id == x.CreatedBy);

                if (staffMember != null)
                    x.StaffName = $"{staffMember.FirstName} {staffMember.LastName}";

                var primaryGuest = users.FirstOrDefault(u => u.Id == DefaultIdType.Parse(x.GuestId));
                if (primaryGuest != null)
                {
                    x.PrimaryGuest = primaryGuest;
                }

                if (x.Items != null)
                    x.Items = x.Items.Select(i =>
                    {
                        if (i.Guests != null)
                            i.Guests = i.Guests.Select(g =>
                            {
                                var guest = users.FirstOrDefault(u => u.Id == g.GuestId);

                                if (guest == null) return g;

                                g.FullName = guest.FullName ?? $"{guest.FirstName} {guest.LastName}";
                                g.DateOfBirth = guest.DateOfBirth;
                                g.Nationality = guest.Nationality;
                                g.Email = guest.Email;
                                g.Gender = guest.Gender;

                                return g;
                            }).ToList();

                        return i;
                    }).ToList();

                return x;
            });

            request.Data = bookings.ToList();
        }

        return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
    }

    private async Task<TourBookingViewModel> GetTourBooking(DefaultIdType id)
    {
        var tourBooking = await BookingsService.GetAsync(id);
        var parsedModel = tourBooking.Adapt<TourBookingViewModel>();

        var toursTask = Task.Run(GetToursAndProperties);
        var guestTask = UserService.GetAsync(parsedModel.GuestId, CancellationToken.None);

        await Task.WhenAll(toursTask, guestTask);

        parsedModel.Tours = toursTask.Result.Item1;
        parsedModel.Properties = toursTask.Result.Item2;
        parsedModel.Categories = toursTask.Result.Item3;
        parsedModel.CurrencyCode = "USD";
        parsedModel.Id = id;
        parsedModel.Guest = guestTask.Result;

        return parsedModel;
    }

    private async Task CreateTourBooking(TourBookingViewModel booking)
    {
        var parsedBooking = booking.Adapt<CreateBookingRequest>();
        parsedBooking.BookingDate = DateTime.Now;
        parsedBooking.GuestId = booking.Guest.Id;
        parsedBooking.GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
        parsedBooking.GuestEmail = booking.Guest.Email;
        parsedBooking.TotalAmount = booking.Items.Sum(item => item.Amount.Value *
                                                              (item.Guests != null && item.Guests.Any()
                                                                  ? item.Guests.Count + 1
                                                                  : 1));

        var bookingId = await BookingsService.CreateAsync(parsedBooking);

        if (bookingId.HasValue)
        {
            await GenerateBookingQrCode(bookingId.Value, booking.Guest.Email);
        }
    }

    private async Task UpdateTourBooking(DefaultIdType id, TourBookingViewModel booking)
    {
        var request = booking.Adapt<UpdateBookingRequest>();
        request.GuestId = booking.Guest.Id;
        request.GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
        request.GuestEmail = booking.Guest.Email;
        request.TotalAmount = booking.Items.Sum(item => item.Amount.Value *
                                                        (item.Guests != null && item.Guests.Any()
                                                            ? item.Guests.Count + 1
                                                            : 1));

        var existingBooking = await BookingsService.GetAsync(id);

        if (existingBooking.TotalAmount != request.TotalAmount && request.IsPaid)
        {
            // Ask if staff want to process refund
            RefundAmount = existingBooking.TotalAmount - request.TotalAmount;

            if (RefundAmount is < 0)
            {
                //Tour has been swapped and guest owes more money, create new stripe payment link
                RefundAmount = Math.Abs(RefundAmount.Value);
                var result = await AdditionalChargeMessageBox.Show();

                if (result != null)
                {
                    request.AmountOutstanding = RefundAmount.Value;
                    request.IsPaid = false;
                    
                    await BookingsService.UpdateAsync(id, request);
                }
            }
            else
            {
                var parameters = new DialogParameters
                {
                    {nameof(PartialRefundConfirmation.RefundAmount), existingBooking.TotalAmount - request.TotalAmount}
                };
                
                var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
                var dialog = await DialogService.ShowAsync<PartialRefundConfirmation>(L["Process Refund"], parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    var refundPercentageAmount = int.Parse(result.Data.ToString() ?? "0");

                    if (refundPercentageAmount > 0)
                    {
                        var percentageAmount = RefundAmount.Value * (refundPercentageAmount / 100.0m);
                    
                        //refund and update
                        await ServiceHelper.ExecuteCallGuardedAsync(
                            async () =>
                            {
                                await BookingsService.RefundBooking(new RefundBookingRequest(id, percentageAmount, false,
                                    true));
                                await BookingsService.UpdateAsync(id, request);
                            },
                            Snackbar,
                            Logger, "Booking Refunded Successfully");

                        _ = _table.ReloadDataAsync();
                    }
                    else
                    {
                        request.DoNotUpdateAmount = true;
                        await BookingsService.UpdateAsync(id, request);
                    }
                }
            }
        }
        else
        {
            await BookingsService.UpdateAsync(id, request);
        }
    }

    private async Task<FileResponse> ExportTourBooking(BaseFilter filter)
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

    // private async Task<ICollection<TourDto>> GetTours()
    // {
    //     var tours = await ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999});
    //     return tours?.Data ?? [];
    // }

    private async Task<Tuple<ICollection<TourDto>, ICollection<PropertyDto>, ICollection<TourCategoryDto>>>
        GetToursAndProperties()
    {
        var toursTask = Task.Run(() =>
            ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999}));

        var propertiesTask = Task.Run(() =>
            PropertiesService.SearchAsync(new SearchPropertiesRequest() {PageNumber = 1, PageSize = 99999}));

        var tourCategoriesTask = Task.Run(() =>
            TourCategoriesService.SearchAsync(new SearchTourCategoriesRequest() {PageNumber = 1, PageSize = 99999}));

        await Task.WhenAll(toursTask, propertiesTask, tourCategoriesTask);

        if (toursTask.Result?.Data != null && propertiesTask.Result?.Data != null)
            return new Tuple<ICollection<TourDto>, ICollection<PropertyDto>, ICollection<TourCategoryDto>>(
                toursTask.Result.Data,
                propertiesTask.Result.Data, tourCategoriesTask.Result.Data);

        return new Tuple<ICollection<TourDto>, ICollection<PropertyDto>, ICollection<TourCategoryDto>>(
            new List<TourDto>(), new List<PropertyDto>(), new List<TourCategoryDto>());
    }

    private void ShowBookingItems(BookingDto request)
    {
        request.ShowDetails = !request.ShowDetails;
    }

    /// <summary>
    /// Generates a Stripe QR Code dialog for Guest to pay for unpaid tour.
    /// </summary>
    /// <param name="bookingId"></param>
    /// <param name="guestEmail"></param>
    private async Task GenerateBookingQrCode(DefaultIdType bookingId, string guestEmail)
    {
        await LoadingService.ToggleLoaderVisibility(true);

        var booking = await BookingsService.GetAsync(bookingId);

        if (booking.IsPaid)
        {
            await LoadingService.ToggleLoaderVisibility(false);
            throw new CustomException("Booking has already been paid, please refresh the page.");
        }

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

    /// <summary>
    /// Generates a Payment Reminder email.
    /// </summary>
    /// <param name="bookingId"></param>
    /// <param name="guestEmail"></param>
    private async Task GeneratePaymentEmail(DefaultIdType bookingId, string guestEmail)
    {
        await LoadingService.ToggleLoaderVisibility(true);

        var booking = await BookingsService.GetAsync(bookingId);

        if (booking.IsPaid)
        {
            await LoadingService.ToggleLoaderVisibility(false);
            throw new CustomException("Booking has already been paid, please refresh the page.");
        }

        var stripeCheckoutUrl = await StripeService.CreateStripeQrCodeUrl(new CreateStripeQrCodeRequest()
        {
            BookingId = booking.Id,
            GuestEmail = guestEmail
        });

        await ServiceHelper.ExecuteCallGuardedAsync(async () =>
            {
                var emailHtml = new ComponentRenderer<EmailTemplates.PaymentReminder>()
                    .Set(c => c.TenantName, TenantInfo.Name)
                    .Set(x => x.PrimaryBackgroundColor, TenantInfo.PrimaryHoverColor)
                    .Set(x => x.HeaderBackgroundColor, TenantInfo.SecondaryHoverColor)
                    .Set(x => x.LogoImageUrl, TenantInfo.LogoImageUrl)
                    .Set(x => x.Booking, booking)
                    .Set(x => x.StripeCheckoutUrl, stripeCheckoutUrl)
                    .Render();

                var mailRequest = new MailRequest(
                    to: [booking.GuestEmail],
                    subject: $"Payment reminder for booking {booking.InvoiceId}",
                    body: emailHtml,
                    displayName: TenantInfo.Name);

                await MailService.SendAsync(mailRequest);
            },
            Snackbar,
            Logger,
            "Email Sent Successfully");

        await LoadingService.ToggleLoaderVisibility(false);
    }

    private async Task InvokeStripeQrCodeUrl(string qrcodeImageUrl)
    {
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(StripeQrCode.QrCodeImageUrl), qrcodeImageUrl},
        };

        var dialog = await DialogService.ShowAsync<StripeQrCode>(string.Empty, parameters, options);

        await dialog.Result;
    }

    private async Task DeleteAndRefund(DefaultIdType bookingId)
    {
        var booking = await BookingsService.GetAsync(bookingId);

        string deleteContent = L["You're sure you want to delete and refund {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            {
                nameof(DeleteConfirmation.ContentText),
                string.Format(deleteContent, Context.EntityName, booking.InvoiceId)
            }
        };
        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadingService.ToggleLoaderVisibility(true);

            await ServiceHelper.ExecuteCallGuardedAsync(
                () => BookingsService.RefundBooking(new RefundBookingRequest(bookingId, booking.TotalAmount, true)),
                Snackbar,
                Logger, "Booking Refunded Successfully");

            _ = _table.ReloadDataAsync();
            await LoadingService.ToggleLoaderVisibility(false);
        }
    }

    private async Task Refund(DefaultIdType bookingId)
    {
        var booking = await BookingsService.GetAsync(bookingId);

        string deleteContent = L["You're sure you want to refund {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            {
                nameof(DeleteConfirmation.ContentText),
                string.Format(deleteContent, Context.EntityName, booking.InvoiceId)
            }
        };
        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await LoadingService.ToggleLoaderVisibility(true);
            await ServiceHelper.ExecuteCallGuardedAsync(
                () => BookingsService.RefundBooking(new RefundBookingRequest(bookingId, booking.TotalAmount, false)),
                Snackbar,
                Logger, "Booking Refunded Successfully");

            _ = _table.ReloadDataAsync();
            await LoadingService.ToggleLoaderVisibility(false);
        }
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

    private DefaultIdType? _searchTourId;

    private DefaultIdType? SearchTourId
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

    private async Task InvokeEditGuestDialog(string id)
    {
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(CreateUpdateGuest.Id), id},
            {nameof(CreateUpdateGuest.EmailRequired), false}
        };

        var dialog = await DialogService.ShowAsync<CreateUpdateGuest>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await _table.ReloadDataAsync();
        }
    }
}

public class TourBookingViewModel : UpdateBookingRequest
{
    //public ICollection<UserDetailsDto>? Guests { get; set; }
    public ICollection<TourDto>? Tours { get; set; }
    public ICollection<PropertyDto>? Properties { get; set; }
    public ICollection<TourCategoryDto>? Categories { get; set; }
    public bool? Refunded { get; set; }
    public DefaultIdType? CreatedBy { get; set; }
}