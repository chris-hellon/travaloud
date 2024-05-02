using Microsoft.AspNetCore.Mvc.RazorPages;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Common.Mailing;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Application.PaymentProcessing.Commands;
using Travaloud.Application.PaymentProcessing.Queries;
using Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

namespace Travaloud.Tenants.SharedRCL.Pages.TravaloudPaymentConfirmation;

public class IndexModel : TravaloudBasePageModel
{
    private readonly IStripeService _stripeService;
    public int? BookingId { get; set; }
    public DateTime? BookingDate { get; set; }
    public BookingDetailsDto? Booking { get; set; }
    
    public IndexModel(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    public async Task<IActionResult> OnGetAsync(string stripeSessionId)
    {
        await OnGetDataAsync();

        if (!string.IsNullOrEmpty(stripeSessionId))
        {
            Logger.Information("Processing Travaloud Payment for stripeSessionId: {StripeSessionId}", stripeSessionId);
            
            try
            {
                var stripeStatus = await _stripeService.GetStripePaymentStatus(
                    new GetStripePaymentStatusRequest(stripeSessionId));
                
                var paymentAuthorizationCode = stripeStatus?.PaymentIntent.LatestChargeId;
                
                Logger.Information("StripeSessionId {StripeSessionId} status is {Status}", stripeSessionId, stripeStatus?.Status);
                
                if (stripeStatus?.Status == "complete")
                {
                    if (string.IsNullOrEmpty(stripeStatus.ClientReferenceId))
                        return await RefundAndFail(null, paymentAuthorizationCode,
                            stripeStatus.PaymentIntentId, 0);
                    
                    var bookingId = Guid.Parse(stripeStatus.ClientReferenceId);
                    Booking = await BookingService.GetAsync(bookingId);
                    
                    if (Booking == null)
                    {
                        return await RefundAndFail(bookingId, paymentAuthorizationCode,
                            stripeStatus.PaymentIntentId, Booking.TotalAmount);
                    }
                    
                    await BookingService.FlagBookingAsPaidAsync(bookingId, new FlagBookingAsPaidRequest()
                    {
                        Id = bookingId
                    });

                    BookingId = Booking.InvoiceId;
                    BookingDate = Booking.BookingDate;
                    
                    Logger.Information("Booking {BookingId} flagged as paid for StripeSessionId {StripeSessionId} ", bookingId, stripeSessionId);

                    if (!Booking.IsPaid)
                    {
                        // Send confirmation email
                        var emailModel = new BookingConfirmationTemplateModel(
                            TenantName,
                            MailSettings?.PrimaryBackgroundColor,
                            MailSettings?.SecondaryBackgroundColor,
                            MailSettings?.HeaderBackgroundColor,
                            MailSettings?.TextColor,
                            MailSettings?.LogoImageUrl,
                            Booking,
                            Booking.InvoiceId,
                            $"{HttpContextAccessor.HttpContext?.Request.Scheme}://{HttpContextAccessor.HttpContext?.Request.Host}{HttpContextAccessor.HttpContext?.Request.PathBase}/contact",
                            stripeStatus.CustomerDetails.Email
                        );

                        var emailHtml =
                            await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                                $"/Pages/EmailTemplates/BookingConfirmation.cshtml", emailModel);

                        var mailRequest = new MailRequest(
                            to: [stripeStatus.CustomerDetails.Email!],
                            subject: $"{TenantName} Order Confirmation",
                            body: emailHtml,
                            bcc: MailSettings?.BccAddress != null ? MailSettings?.BccAddress.ToList() : []);

                        await MailService.SendAsync(mailRequest);
                    
                        Logger.Information("Confirmation email sent for BookingId {BookingId}", bookingId);
                    }
                    
                    return Page();
                }
            }
            catch (Exception ex)
            {
                //ignored
                Logger.Error(ex.Message);
            }
        }
        
        return LocalRedirect("/order-failed");
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