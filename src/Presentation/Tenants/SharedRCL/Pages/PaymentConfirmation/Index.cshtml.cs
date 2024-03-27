using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;

namespace Travaloud.Tenants.SharedRCL.Pages.PaymentConfirmation;

public class IndexModel : TravaloudBasePageModel
{
    public int? BookingId { get; set; }
    public DateTime? BookingDate { get; set; }

    public BasketModel? Basket { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? bookingId)
    {
        await OnGetDataAsync();

        Basket = await BasketService.GetBasket();

        if (Basket == null || !bookingId.HasValue) return LocalRedirect("/");

        await BookingService.FlagBookingAsPaidAsync(bookingId.Value, new FlagBookingAsPaidRequest()
        {
            Id = bookingId.Value
        });
        
        var booking = await BookingService.GetAsync(bookingId.Value);
        
        BookingId = booking.InvoiceId;
        BookingDate = booking.BookingDate;
        BasketService.EmptyBasket();
        
        return Page();
    }
}