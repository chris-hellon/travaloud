using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Validation;

public class CreateServiceRequestValidator : CustomValidator<CreateServiceRequest>
{
    public CreateServiceRequestValidator(IRepositoryFactory<Service> repo, IStringLocalizer<CreateServiceRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await repo.SingleOrDefaultAsync(new ServiceByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["service.alreadyexists"], name));

        RuleFor(p => p.ServiceFields)
            .NotEmpty();
    }
}