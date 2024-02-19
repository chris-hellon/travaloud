using Travaloud.Application.Catalog.Properties.Commands;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class CreatePropertyFacilityRequestValidator : CustomValidator<CreatePropertyFacilityRequest>
{
    public CreatePropertyFacilityRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty();
    }
}