using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Validation;

public class CreatePartnerRequestValidator : CustomValidator<CreatePartnerRequest>
{
    public CreatePartnerRequestValidator(IRepositoryFactory<Partner> repo, IStringLocalizer<CreatePartnerRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await repo.SingleOrDefaultAsync(new PartnerByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["supplier.alreadyexists"], name));

        RuleFor(p => p.Address).NotEmpty();

        RuleFor(p => p.City).NotEmpty();

        RuleFor(p => p.PrimaryContactName).NotEmpty();

        RuleFor(p => p.PrimaryContactNumber).NotEmpty();

        RuleFor(p => p.PrimaryEmailAddress).NotEmpty();
    }
}