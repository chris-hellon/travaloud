using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;
using Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

namespace Travaloud.Tenants.SharedRCL.Pages.PaymentConfirmation;

public class IndexModel : TravaloudBasePageModel
{
    private readonly IStripeService _stripeService;
    private readonly ICloudbedsService _cloudbedsService;
    private readonly IPaymentConfirmationService _paymentConfirmationService;
    
    public IndexModel(IStripeService stripeService, ICloudbedsService cloudbedsService, IPaymentConfirmationService paymentConfirmationService)
    {
        _stripeService = stripeService;
        _cloudbedsService = cloudbedsService;
        _paymentConfirmationService = paymentConfirmationService;
    }

    public int? BookingId { get; set; }
    public DateTime? BookingDate { get; set; }

    public BasketModel? Basket { get; set; }

    public async Task<IActionResult> OnGetAsync(string stripeSessionId)
    {
        await OnGetDataAsync();

        Basket = await BasketService.GetBasket();

        if (Basket == null || !Basket.Items.Any()) return LocalRedirect("/");

        Guid? bookingId = null;
        var paymentAuthorizationCode = string.Empty;
        var paymentIntentId = string.Empty;

        if (string.IsNullOrEmpty(stripeSessionId))
            return LocalRedirect("/order-failed");

        try
        {
            var stripeStatus = await _stripeService.GetStripePaymentStatus(
                new GetStripePaymentStatusRequest(stripeSessionId));

            if (stripeStatus == null)
                return LocalRedirect("/order-failed");
            
            paymentIntentId = stripeStatus.PaymentIntentId;
            var cardToken = stripeStatus.CustomerId; 
            paymentAuthorizationCode = stripeStatus.PaymentIntent.LatestChargeId;
            
            if (string.IsNullOrEmpty(paymentIntentId) || string.IsNullOrEmpty(cardToken) || string.IsNullOrEmpty(paymentAuthorizationCode))
                return await RefundAndFail(bookingId, paymentAuthorizationCode,
                    stripeStatus.PaymentIntentId, Basket.Total);
                
            if (stripeStatus.Status == "complete")
            { 
                var guestId = Guid.Parse(stripeStatus.ClientReferenceId);

                var createBookingRequest = await _paymentConfirmationService.CreateBookingRequest(guestId, Basket);
                bookingId = await BookingService.CreateAsync(createBookingRequest);

                if (!bookingId.HasValue)
                {
                    return await RefundAndFail(bookingId, paymentAuthorizationCode,
                        stripeStatus.PaymentIntentId, Basket.Total);
                }
                    
                var booking = await BookingService.GetAsync(bookingId.Value);

                var propertyBookingsProcessed = await _paymentConfirmationService.ProcessPropertyBookings(
                    Basket,
                    booking,
                    cardToken,
                    paymentAuthorizationCode,
                    TenantWebsiteService,
                    _cloudbedsService,
                    BookingService
                );
                
                if (!propertyBookingsProcessed)
                    return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                        stripeStatus.PaymentIntentId, Basket.Total);
                
                await BookingService.FlagBookingAsPaidAsync(bookingId.Value, new FlagBookingAsPaidRequest()
                {
                    Id = bookingId.Value
                });

                BookingId = booking.InvoiceId;
                BookingDate = booking.BookingDate;

                // Send confirmation email
                var emailModel = new BookingConfirmationTemplateModel(
                    TenantName,
                    MailSettings.PrimaryBackgroundColor,
                    MailSettings.SecondaryBackgroundColor,
                    MailSettings.HeaderBackgroundColor,
                    MailSettings.TextColor,
                    MailSettings.LogoImageUrl,
                    Basket,
                    booking.InvoiceId,
                    $"{HttpContextAccessor.HttpContext?.Request.Scheme}://{HttpContextAccessor.HttpContext?.Request.Host}{HttpContextAccessor.HttpContext?.Request.PathBase}/contact"
                );

                var emailHtml =
                    await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                        $"/Pages/EmailTemplates/BookingConfirmation.cshtml", emailModel);

                var mailRequest = new MailRequest(
                    to: [Basket.Email!],
                    subject: $"{TenantName} Order Confirmation",
                    body: emailHtml,
                    bcc: (MailSettings.BccAddress != null ? MailSettings.BccAddress.ToList() : [])!);

                await MailService.SendAsync(mailRequest);

                if (!(bool) HttpContextAccessor.HttpContext?.Session.Keys.Contains("GuestId"))
                {
                    HttpContextAccessor.HttpContext?.Session.Remove("GuestId");
                }

                BasketService.EmptyBasket();

                return Page();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            StatusSeverity = "danger";
        }

        return await RefundAndFail(bookingId, paymentAuthorizationCode,
            paymentIntentId, Basket.Total);
    }

    private async Task<IActionResult> RefundAndFail(
        Guid? bookingId,
        string? paymentAuthorizationCode,
        string? paymentIntentId,
        decimal total)
    {
        await _stripeService.RefundSession(new RefundSessionRequest(paymentAuthorizationCode, paymentIntentId, total));

        if (bookingId.HasValue)
            await BookingService.DeleteAsync(bookingId.Value);

        return LocalRedirect("/order-failed");
    }
}