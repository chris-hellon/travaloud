using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Validation;

public class UpdatePartnerRequestValidator : CustomValidator<UpdatePartnerRequest>
{
    public UpdatePartnerRequestValidator(IRepositoryFactory<Partner> repo, IStringLocalizer<UpdatePartnerRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (property, name, ct) =>
                await repo.SingleOrDefaultAsync(new PartnerByNameSpec(name), ct)
                    is not Partner existingProduct || existingProduct.Id == property.Id)
            .WithMessage((_, name) => string.Format(localizer["partner.alreadyexists"], name));

        RuleFor(p => p.Address).NotEmpty();

        RuleFor(p => p.City).NotEmpty();

        RuleFor(p => p.PrimaryContactName).NotEmpty();

        RuleFor(p => p.PrimaryContactNumber).NotEmpty();

        RuleFor(p => p.PrimaryEmailAddress).NotEmpty();
    }
}