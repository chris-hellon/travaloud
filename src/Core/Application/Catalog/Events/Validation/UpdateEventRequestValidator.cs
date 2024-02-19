using Travaloud.Application.Catalog.Events.Commands;

namespace Travaloud.Application.Catalog.Events.Validation;

public class UpdateEventRequestValidator : CustomValidator<UpdateEventRequest>
{
    public UpdateEventRequestValidator()
    {
            RuleFor(p => p.Image)
                .SetNonNullableValidator(new FileUploadRequestValidator());
        }
}