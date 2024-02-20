using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdateBookingDirectionContentValidator : CustomValidator<PropertyDirectionContentRequest>
{
    public UpdateBookingDirectionContentValidator()
    {
        RuleFor(p => p.Body).NotEmpty();
    }
}