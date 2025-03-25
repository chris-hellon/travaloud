using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Application.Catalog.Bookings.Validation;

public class CreateBookingRequestValidator : CustomValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator(IRepositoryFactory<Booking> repo)
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

        RuleFor(b => b)
            .MustAsync(async (request, cancellationToken) =>
            {
                var existingBooking = await repo.SingleOrDefaultAsync(
                    new BookingByGuestIdAndDateSpec(request.GuestId, request.BookingDate, request.Description), cancellationToken);

                return existingBooking is null;
            })
            .WithMessage("A booking with the same Guest Id, Booking Date, and Description already exists.");

        RuleFor(b => b.Items)
            .NotEmpty()
            .Must(ValidateItems)
            .WithMessage("One or more items have validation errors.");
    }

    private bool ValidateItems(IEnumerable<CreateBookingItemRequest> items)
    {
        var validator = new CreateBookingItemRequestValidator();
        var validationResults = items.Select(item => validator.Validate(item));
        var aggregateErrors = validationResults.SelectMany(result => result.Errors).ToList();
        if (aggregateErrors.Any())
        {
            foreach (var error in aggregateErrors)
            {
                error.PropertyName = $"Items[{error.PropertyName}]";
            }

            throw new ValidationException(aggregateErrors);
        }

        return true;
    }
}