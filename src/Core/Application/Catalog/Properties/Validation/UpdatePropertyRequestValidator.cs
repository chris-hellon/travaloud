using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Validation;

public class UpdatePropertyRequestValidator : CustomValidator<UpdatePropertyRequest>
{
    public UpdatePropertyRequestValidator(IRepositoryFactory<Property> repo, IStringLocalizer<UpdatePropertyRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (property, name, ct) =>
                await repo.SingleOrDefaultAsync(new PropertyByNameSpec(name), ct)
                    is not Property existingProduct || existingProduct.Id == property.Id)
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