using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdateBookingDirectionContentValidator : CustomValidator<UpdatePropertyDirectionContentRequest>
{
    public UpdateBookingDirectionContentValidator()
    {
        RuleFor(p => p.Body).NotEmpty();
    }
}