using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Validation;

public class CreateServiceFieldRequestValidator : CustomValidator<CreateServiceFieldRequest>
{
    public CreateServiceFieldRequestValidator(IRepositoryFactory<ServiceField> repo, IStringLocalizer<CreateServiceFieldRequestValidator> localizer)
    {
        RuleFor(p => p.Label)
            .NotEmpty()
            .MaximumLength(1024);

        RuleFor(p => p.FieldType)
            .NotEmpty();
        
        RuleFor(p => p.Width)
            .NotEmpty();
    }
}