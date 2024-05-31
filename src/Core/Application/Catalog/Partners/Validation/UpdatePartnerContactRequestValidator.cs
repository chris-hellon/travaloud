using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Validation;

public class UpdatePartnerContactRequestValidator : CustomValidator<UpdatePartnerContactRequest>
{
    public UpdatePartnerContactRequestValidator(IRepositoryFactory<PartnerContact> repo, IStringLocalizer<UpdatePartnerRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (property, name, ct) =>
                await repo.SingleOrDefaultAsync(new PartnerContactByNameSpec(name), ct)
                    is not PartnerContact existingProduct || existingProduct.Id == property.Id)
            .WithMessage((_, name) => string.Format(localizer["supplierContact.alreadyexists"], name));
        
        RuleFor(p => p.ContactNumber).NotEmpty();

        RuleFor(p => p.EmailAddress).NotEmpty();
    }
}