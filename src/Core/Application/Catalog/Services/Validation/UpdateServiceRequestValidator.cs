using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Validation;

public class UpdateServiceRequestValidator : CustomValidator<UpdateServiceRequest>
{
    public UpdateServiceRequestValidator(IRepositoryFactory<Service> repo, IStringLocalizer<UpdateServiceRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (property, name, ct) =>
                await repo.SingleOrDefaultAsync(new ServiceByNameSpec(name), ct)
                    is not Service existingProduct || existingProduct.Id == property.Id)
            .WithMessage((_, name) => string.Format(localizer["service.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.ServiceFields)
            .NotEmpty();
    }
}