using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class CreatePropertyRequestValidator : CustomValidator<CreatePropertyRequest>
{
    public CreatePropertyRequestValidator(IRepositoryFactory<Property> repo, IStringLocalizer<CreatePropertyRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await repo.SingleOrDefaultAsync(new PropertyByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["property.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.AddressLine1)
            .NotEmpty();

        RuleFor(p => p.TelephoneNumber)
            .NotEmpty();

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}