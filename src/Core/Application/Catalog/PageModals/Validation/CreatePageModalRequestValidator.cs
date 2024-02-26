using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Application.Catalog.PageModals.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Validation;

public class CreatePageModalRequestValidator : CustomValidator<CreatePageModalRequest>
{
    public CreatePageModalRequestValidator(IRepositoryFactory<PageModal> repository, IStringLocalizer<CreatePageModalRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => title != null && await repository.SingleOrDefaultAsync(new PageModalByTitleSpec(title), ct) is null)
            .WithMessage((_, title) => string.Format(localizer["pagemodal.alreadyexists"], title));

        RuleFor(x => x.SelectedPages)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x._validationSelectedPages)
            .NotNull().WithMessage("'Assigned Pages' must not be empty.");
    }
}