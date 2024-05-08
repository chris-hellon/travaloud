using System.Drawing.Imaging;
using BlazorTemplater;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QRCoder;
using Travaloud.Admin.Components.Dialogs;
using Travaloud.Admin.Components.Dialogs.Bookings;
using Travaloud.Admin.Components.EntityTable;
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
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Pages.Bookings;

public partial class TourBookings
{
    [Inject] protected IBookingsService BookingsService { get; set; } = default!;

    [Inject] protected IPropertiesService PropertiesService { get; set; } = default!;

    [Inject] protected IToursService ToursServive { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Inject] protected IStripeService StripeService { get; set; } = default!;

    [Inject] protected IEmailTemplateService EmailTemplateService { get; set; } = default!;

    [Inject] protected IMailService MailService { get; set; } = default!;

    [Parameter] public string Category { get; set; } = default!;

    [CascadingParameter] private TravaloudTenantInfo? TenantInfo { get; set; }
    
    [CascadingParameter] private MudTheme? CurrentTheme { get; set; }

    private EntityServerTableContext<BookingDto, Guid, TourBookingViewModel> Context { get; set; } = default!;

    private EntityTable<BookingDto, Guid, TourBookingViewModel> _table = default!;

    private MudDateRangePicker _bookingDateRangePicker = default!;

    private MudDateRangePicker _tourDateRangePicker = default!;

    private ICollection<TourDto> Tours { get; set; } = default!;

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
                new EntityField<BookingDto>(booking => booking.BookingDate, L["Booking Date"], "BookingDate"),
                new EntityField<BookingDto>(booking => $"$ {booking.TotalAmount:n2}", L["Total Amount"], "TotalAmount"),
                new EntityField<BookingDto>(booking => booking.IsPaid, L["Is Paid"], "IsPaid", Color: booking => !booking.IsPaid ? CurrentTheme.Palette.Error : null),
                new EntityField<BookingDto>(booking => booking.WaiverSigned ?? false, L["Waiver Signed"], "WaiverSigned")
            ],
            enableAdvancedSearch: false,
            canViewEntityFunc: booking => booking.IsPaid,
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

        var staffIds = request.Data.Select(x => x.CreatedBy.ToString()).ToList();

        var staff = await UserService.SearchAsync(staffIds, CancellationToken.None);

        if (staff.Count == 0) return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
        {
            var bookings = request.Data.Select(x =>
            {
                var staffMember = staff.FirstOrDefault(s => s.Id == x.CreatedBy);

                if (staffMember != null)
                    x.StaffName = $"{staffMember.FirstName} {staffMember.LastName}";
                return x;
            });

            request.Data = bookings.ToList();
        }

        return request.Adapt<EntityTable.PaginationResponse<BookingDto>>();
    }

    private async Task<TourBookingViewModel> GetTourBooking(Guid id)
    {
        var tourBooking = await BookingsService.GetAsync(id);
        var parsedModel = tourBooking.Adapt<TourBookingViewModel>();

        var toursTask = Task.Run(GetToursAndProperties);
        var guestTask = UserService.GetAsync(parsedModel.GuestId, CancellationToken.None);

        await Task.WhenAll(toursTask, guestTask);

        parsedModel.Tours = toursTask.Result.Item1;
        parsedModel.Properties = toursTask.Result.Item2;
        parsedModel.CurrencyCode = "USD";
        parsedModel.Id = id;
        parsedModel.Guest = guestTask.Result;

        return parsedModel;
    }

    private async Task CreateTourBooking(TourBookingViewModel booking)
    {
        var parsedBooking = booking.Adapt<CreateBookingRequest>();
        parsedBooking.BookingDate = DateTime.Now;
        parsedBooking.GuestId = booking.Guest.Id.ToString();
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

    private async Task UpdateTourBooking(Guid id, TourBookingViewModel booking)
    {
        var request = booking.Adapt<UpdateBookingRequest>();
        request.GuestId = booking.Guest.Id;
        request.GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}";
        request.GuestEmail = booking.Guest.Email;
        request.TotalAmount = booking.Items.Sum(item => item.Amount.Value *
                                                        (item.Guests != null && item.Guests.Any()
                                                            ? item.Guests.Count + 1
                                                            : 1));
        await BookingsService.UpdateAsync(id, request);
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

    private async Task<Tuple<ICollection<TourDto>, ICollection<PropertyDto>>> GetToursAndProperties()
    {
        var toursTask = Task.Run(() =>
            ToursServive.SearchAsync(new SearchToursRequest() {PageNumber = 1, PageSize = 99999}));

        var propertiesTask = Task.Run(() =>
            PropertiesService.SearchAsync(new SearchPropertiesRequest() {PageNumber = 1, PageSize = 99999}));

        await Task.WhenAll(toursTask, propertiesTask);

        if (toursTask.Result?.Data != null && propertiesTask.Result?.Data != null)
            return new Tuple<ICollection<TourDto>, ICollection<PropertyDto>>(toursTask.Result.Data,
                propertiesTask.Result.Data);

        return new Tuple<ICollection<TourDto>, ICollection<PropertyDto>>(new List<TourDto>(), new List<PropertyDto>());
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
    private async Task GenerateBookingQrCode(Guid bookingId, string guestEmail)
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
    private async Task GeneratePaymentEmail(Guid bookingId, string guestEmail)
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

    private async Task DeleteAndRefund(Guid bookingId)
    {
        var booking = await BookingsService.GetAsync(bookingId);

        string deleteContent = L["You're sure you want to delete and refund {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, Context.EntityName, booking.InvoiceId)}
        };
        var options = new DialogOptions {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            await ServiceHelper.ExecuteCallGuardedAsync(
                () => BookingsService.RefundBooking(new RefundBookingRequest(booking)),
                Snackbar,
                Logger, "Booking Refunded Successfully");

            _ = _table.ReloadDataAsync();
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
}

public class TourBookingViewModel : UpdateBookingRequest
{
    //public ICollection<UserDetailsDto>? Guests { get; set; }
    public ICollection<TourDto>? Tours { get; set; }
    public ICollection<PropertyDto>? Properties { get; set; }
}