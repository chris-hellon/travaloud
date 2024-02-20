using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Application.Catalog.PageModals.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Validation;

public class UpdatePageModalRequestValidator : CustomValidator<UpdatePageModalRequest>
{
    public UpdatePageModalRequestValidator(IRepositoryFactory<PageModal> repo, IStringLocalizer<UpdatePageModalRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (pageModal, name, ct) =>
                await repo.SingleOrDefaultAsync(new PageModalByTitleSpec(name), ct)
                    is not { } existingPageModal || existingPageModal.Id == pageModal.Id)
            .WithMessage((_, name) => string.Format(localizer["pagemodal.alreadyexists"], name));

        RuleFor(x => x.SelectedPages)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.ValidationSelectedPages)
            .NotNull().WithMessage("'Assigned Pages' must not be empty.");
    }
}