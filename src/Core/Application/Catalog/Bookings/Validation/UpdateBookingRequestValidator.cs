using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Validation;

public class UpdateBookingRequestValidator : CustomValidator<UpdateBookingRequest>
{
    public UpdateBookingRequestValidator(IRepositoryFactory<Booking> repo)
    {
        RuleFor(b => b.Description)
            .NotEmpty();

        RuleFor(b => b.TotalAmount)
            .GreaterThan(0);

        RuleFor(b => b.CurrencyCode)
            .NotEmpty();

        RuleFor(b => b.ItemQuantity)
            .GreaterThan(0);

        RuleFor(b => b.GuestId)
            .NotEmpty();

        RuleFor(b => b.Items)
            .NotEmpty()
            .ForEach(itemRule => itemRule.SetValidator(new UpdateBookingItemRequestValidator()));
    }
}