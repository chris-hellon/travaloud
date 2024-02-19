using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Validation;


public class UpdateServiceFieldRequestValidator : CustomValidator<UpdateServiceFieldRequest>
{
    public UpdateServiceFieldRequestValidator(IRepositoryFactory<ServiceField> repo, IStringLocalizer<UpdateServiceFieldRequestValidator> localizer)
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