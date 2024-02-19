using Travaloud.Application.Catalog.Events.Commands;

namespace Travaloud.Application.Catalog.Events.Validation;

public class CreateEventRequestValidator : CustomValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
            RuleFor(p => p.Image)
                .SetNonNullableValidator(new FileUploadRequestValidator());
        }
}