using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Validation;

public class UpdateBookingRequestValidator : CustomValidator<UpdateBookingRequest>
{
    public UpdateBookingRequestValidator(IRepositoryFactory<Booking> repo)
    {
        RuleFor(b => b.Description)
            .NotEmpty()
            .WithMessage("Please enter a Description.");

        RuleFor(b => b.TotalAmount)
            .GreaterThan(0);

        RuleFor(b => b.CurrencyCode)
            .NotEmpty();

        RuleFor(b => b.ItemQuantity)
            .GreaterThan(0);
        
        RuleFor(b => b.Guest)
            .NotEmpty()
            .WithMessage("Please select a Primary Guest.");

        RuleFor(b => b.BookingSource)
            .NotEmpty()
            .WithMessage("Please select a Booking Source.");
        
        RuleFor(b => b.Items)
            .NotEmpty()
            .ForEach(itemRule => itemRule.SetValidator(new UpdateBookingItemRequestValidator()));

    }
}