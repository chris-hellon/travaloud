using Travaloud.Application.Catalog.Bookings.Commands;

namespace Travaloud.Application.Catalog.Bookings.Validation;

public class BookingItemGuestRequestValidator : CustomValidator<BookingItemGuestRequest>
{
    public BookingItemGuestRequestValidator()
    {
        RuleFor(b => b.Guest)
            .NotEmpty().WithMessage("Please select a Guest.");
    }
}