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
            .NotEmpty();

        RuleFor(b => b.TourDateId).NotEmpty();
        
        RuleFor(b => b.PickupLocation).NotEmpty();
    }
}