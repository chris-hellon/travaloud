using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Validation;

public class CreatePartnerContactRequestValidator : CustomValidator<CreatePartnerContactRequest>
{
    public CreatePartnerContactRequestValidator(IRepositoryFactory<PartnerContact> repo, IStringLocalizer<CreatePartnerRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await repo.SingleOrDefaultAsync(new PartnerContactByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["supplierContact.alreadyexists"], name));
        
        RuleFor(p => p.ContactNumber).NotEmpty();

        RuleFor(p => p.EmailAddress).NotEmpty();
    }
}