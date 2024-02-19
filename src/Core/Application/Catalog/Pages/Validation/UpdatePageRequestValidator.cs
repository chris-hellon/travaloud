using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Validation;

public class UpdatePageRequestValidator : CustomValidator<UpdatePageRequest>
{
    public UpdatePageRequestValidator(IRepositoryFactory<Page> repo, IStringLocalizer<UpdatePageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (pageModal, name, ct) =>
                await repo.SingleOrDefaultAsync(new PageByTitleSpec(name), ct)
                    is not { } existingPageModal || existingPageModal.Id == pageModal.Id)
            .WithMessage((_, name) => string.Format(localizer["page.alreadyexists"], name));
    }
}

