using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Commands;
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

    public IndexModel(IStripeService stripeService, ICloudbedsService cloudbedsService)
    {
        _stripeService = stripeService;
        _cloudbedsService = cloudbedsService;
    }

    public int? BookingId { get; set; }
    public DateTime? BookingDate { get; set; }

    public BasketModel? Basket { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? bookingId)
    {
        await OnGetDataAsync();

        Basket = await BasketService.GetBasket();

        if (Basket == null || !bookingId.HasValue) return LocalRedirect("/");

        var booking = await BookingService.GetAsync(bookingId.Value);

        if (!string.IsNullOrEmpty(booking.StripeSessionId))
        {
            try
            {
                var stripeStatus =
                    await _stripeService.GetStripePaymentStatus(
                        new GetStripePaymentStatusRequest(booking.StripeSessionId));

                var properties = await TenantWebsiteService.GetProperties(CancellationToken.None);

                if (stripeStatus?.Status == "complete")
                {
                    var cardToken = stripeStatus.CustomerId;
                    var paymentAuthorizationCode = stripeStatus.PaymentIntent.LatestChargeId;

                    if (properties == null)
                        return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                            stripeStatus.PaymentIntentId, Basket.Total);

                    if (Basket.Items.Any(x => x.PropertyId.HasValue))
                    {
                        // Create cloudbeds reservation
                        foreach (var basketItem in Basket.Items.Where(x => x.PropertyId.HasValue))
                        {
                            var property = properties.FirstOrDefault(x =>
                                basketItem.PropertyId != null && x.Id == basketItem.PropertyId.Value);

                            if (property == null)
                                return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                                    stripeStatus.PaymentIntentId, Basket.Total);

                            string? reservationId;

                            try
                            {
                                var createReservationResponse =
                                    await _cloudbedsService.CreateReservation(new CreateReservationRequest(Basket,
                                        basketItem, property.CloudbedsApiKey, cardToken, paymentAuthorizationCode));

                                if (!createReservationResponse.Success || booking.Items == null)
                                    return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                                        stripeStatus.PaymentIntentId, Basket.Total);

                                reservationId = createReservationResponse.ReservationID;
                            }
                            catch (Exception)
                            {
                                return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                                    stripeStatus.PaymentIntentId, Basket.Total);
                            }

                            var bookingItem = booking.Items.FirstOrDefault(x =>
                                basketItem is {CheckOutDateParsed: not null, CheckInDateParsed: not null} &&
                                x.PropertyId == basketItem.PropertyId &&
                                x.StartDate.Date == basketItem.CheckInDateParsed.Value.Date &&
                                x.EndDate.Date == basketItem.CheckOutDateParsed.Value.Date);

                            if (bookingItem != null && !string.IsNullOrEmpty(reservationId))
                            {
                                // Update booking item with reservation id
                                await BookingService.UpdateBookingItemReservation(bookingItem.Id,
                                    new UpdateBookingItemReservationIdRequest(bookingItem.Id, reservationId));
                            }

                            if (basketItem.Guests == null || !basketItem.Guests.Any()) continue;

                            var additionalGuestsTasks = basketItem.Guests.Select(guest =>
                                    _cloudbedsService.CreateReservationAdditionalGuest(new
                                        CreateReservationAdditionalGuestRequest(
                                            basketItem.CloudbedsPropertyId,
                                            reservationId,
                                            guest.FirstName,
                                            guest.Surname,
                                            guest.Gender,
                                            guest.Email,
                                            guest.PhoneNumber,
                                            guest.DateOfBirth,
                                            property.CloudbedsApiKey)))
                                .Cast<Task>()
                                .ToList();

                            await Task.WhenAll(additionalGuestsTasks);
                        }
                    }

                    await BookingService.FlagBookingAsPaidAsync(bookingId.Value, new FlagBookingAsPaidRequest()
                    {
                        Id = bookingId.Value
                    });

                    BookingId = booking.InvoiceId;
                    BookingDate = booking.BookingDate;

                    // Send confirmation email
                    var emailModel = new BookingConfirmationTemplateModel(
                        TenantName,
                        MailSettings?.PrimaryBackgroundColor,
                        MailSettings?.SecondaryBackgroundColor,
                        MailSettings?.HeaderBackgroundColor,
                        MailSettings?.TextColor,
                        MailSettings?.LogoImageUrl,
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
                        bcc: [MailSettings?.BccAddress?.ToString()]);

                    await MailService.SendAsync(mailRequest);

                    BasketService.EmptyBasket();

                    return Page();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        await BookingService.DeleteAsync(bookingId.Value);

        return LocalRedirect("/order-failed");
    }

    private async Task<IActionResult> RefundAndFail(Guid bookingId, string paymentAuthorizationCode,
        string paymentIntentId, decimal total)
    {
        await _stripeService.RefundSession(new RefundSessionRequest(paymentAuthorizationCode, paymentIntentId, total));
        await BookingService.DeleteAsync(bookingId);
        return LocalRedirect("/order-failed");
    }
}