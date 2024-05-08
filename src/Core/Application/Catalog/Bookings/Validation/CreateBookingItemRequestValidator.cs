using Travaloud.Application.Catalog.Bookings.Commands;

namespace Travaloud.Application.Catalog.Bookings.Validation;

public class CreateBookingItemRequestValidator : CustomValidator<CreateBookingItemRequest>
{
    public CreateBookingItemRequestValidator()
    {
        RuleFor(i => i.StartDate)
            .NotEmpty();

        RuleFor(i => i.EndDate)
            .NotEmpty();

        RuleFor(i => i.Amount)
            .GreaterThan(0);

        RuleFor(i => i.RoomQuantity)
            .GreaterThan(0);

        RuleFor(i => i.TourId)
            .NotEmpty()
            .WithMessage("Please select a Tour.");

        RuleFor(b => b.TourDateId)
            .NotEmpty()
            .WithMessage("Please select a Tour Date.");
        
        RuleFor(b => b.PickupLocation)
            .NotEmpty()
            .WithMessage("Please select a Pick Up Location.");
    }
}