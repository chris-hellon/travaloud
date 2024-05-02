using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class CreateTourCategoryRequestValidator : CustomValidator<CreateTourCategoryRequest>
{
    public CreateTourCategoryRequestValidator(IRepositoryFactory<TourCategory> tourRepo, IStringLocalizer<CreateTourCategoryRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await tourRepo.SingleOrDefaultAsync(new TourCategoryByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["tourCategory.alreadyexists"], name));

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}