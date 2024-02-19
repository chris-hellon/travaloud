using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class CreateTourRequestValidator : CustomValidator<CreateTourRequest>
{
    public CreateTourRequestValidator(IRepositoryFactory<Tour> tourRepo, IStringLocalizer<CreateTourRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await tourRepo.SingleOrDefaultAsync(new TourByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["tour.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.MinCapacity).
            NotEmpty();

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}