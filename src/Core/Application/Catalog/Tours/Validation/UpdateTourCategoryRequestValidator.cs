using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class UpdateTourCategoryRequestValidator : CustomValidator<UpdateTourCategoryRequest>
{
    public UpdateTourCategoryRequestValidator(IRepositoryFactory<TourCategory> tourRepo, IStringLocalizer<UpdateTourCategoryRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (tour, name, ct) =>
                await tourRepo.SingleOrDefaultAsync(new TourCategoryByNameSpec(name), ct)
                    is not TourCategory existingTour || existingTour.Id == tour.Id)
            .WithMessage((_, name) => string.Format(localizer["tourCategory.alreadyexists"], name));
        
        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}