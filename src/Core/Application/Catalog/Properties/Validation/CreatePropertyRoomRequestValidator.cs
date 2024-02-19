using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class CreatePropertyRoomRequestValidator : CustomValidator<CreatePropertyRoomRequest>
{
    public CreatePropertyRoomRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.ShortDescription)
            .NotEmpty();

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}