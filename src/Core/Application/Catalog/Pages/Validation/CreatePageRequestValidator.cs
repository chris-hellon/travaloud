using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Validation;

public class CreatePageRequestValidator : CustomValidator<CreatePageRequest>
{
    public CreatePageRequestValidator(IRepositoryFactory<Page> repository, IStringLocalizer<CreatePageRequestValidator> localizer)
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .MustAsync(async (title, ct) => await repository.SingleOrDefaultAsync(new PageByTitleSpec(title), ct) is null)
            .WithMessage((_, title) => string.Format(localizer["page.alreadyexists"], title));
    }
}