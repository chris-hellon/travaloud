using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class CreatePropertyDirectionContentRequestValidator : CustomValidator<CreatePropertyDirectionContentRequest>
{
    public CreatePropertyDirectionContentRequestValidator()
    {
        RuleFor(p => p.Body).NotEmpty();
    }
}