using Microsoft.AspNetCore.Mvc.RazorPages;
using Travaloud.Application.Catalog.Bookings.Commands;
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
    
    public IndexModel(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    public async Task<IActionResult> OnGetAsync(string stripeSessionId)
    {
        await OnGetDataAsync();

        if (!string.IsNullOrEmpty(stripeSessionId))
        {
            try
            {
                var stripeStatus = await _stripeService.GetStripePaymentStatus(
                    new GetStripePaymentStatusRequest(stripeSessionId));
                
                var paymentIntentId = stripeStatus.PaymentIntentId;
                var cardToken = stripeStatus.CustomerId; 
                var paymentAuthorizationCode = stripeStatus.PaymentIntent.LatestChargeId;
                
                if (stripeStatus?.Status == "complete")
                {
                    if (string.IsNullOrEmpty(stripeStatus.ClientReferenceId))
                        return await RefundAndFail(null, paymentAuthorizationCode,
                            stripeStatus.PaymentIntentId, 0);
                    
                    var bookingId = Guid.Parse(stripeStatus.ClientReferenceId);
                    var booking = await BookingService.GetAsync(bookingId);
                    
                    if (booking == null)
                    {
                        return await RefundAndFail(bookingId, paymentAuthorizationCode,
                            stripeStatus.PaymentIntentId, booking.TotalAmount);
                    }
                    
                    await BookingService.FlagBookingAsPaidAsync(bookingId, new FlagBookingAsPaidRequest()
                    {
                        Id = bookingId
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
                        booking,
                        booking.InvoiceId,
                        $"{HttpContextAccessor.HttpContext?.Request.Scheme}://{HttpContextAccessor.HttpContext?.Request.Host}{HttpContextAccessor.HttpContext?.Request.PathBase}/contact",
                        stripeStatus.CustomerEmail
                    );

                    var emailHtml =
                        await RazorPartialToStringRenderer.RenderPartialToStringAsync(
                            $"/Pages/EmailTemplates/BookingConfirmation.cshtml", emailModel);

                    var mailRequest = new MailRequest(
                        to: [stripeStatus.CustomerEmail!],
                        subject: $"{TenantName} Order Confirmation",
                        body: emailHtml,
                        bcc: MailSettings?.BccAddress != null ? MailSettings?.BccAddress.ToList() : []);

                    await MailService.SendAsync(mailRequest);
                    
                    return Page();
                }
            }
            catch (Exception ex)
            {
                //ignored
            }
        }
        
        return LocalRedirect("/order-failed");
    }
    
    private async Task<IActionResult> RefundAndFail(
        Guid? bookingId,
        string paymentAuthorizationCode,
        string paymentIntentId,
        decimal total)
    {
        await _stripeService.RefundSession(new RefundSessionRequest(paymentAuthorizationCode, paymentIntentId, total));

        if (bookingId.HasValue)
            await BookingService.DeleteAsync(bookingId.Value);

        return LocalRedirect("/order-failed");
    }
}