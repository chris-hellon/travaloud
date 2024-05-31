using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Validation;

public class UpdateTourRequestValidator : CustomValidator<UpdateTourRequest>
{
    public UpdateTourRequestValidator(IRepositoryFactory<Tour> tourRepo, IStringLocalizer<UpdateTourRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (tour, name, ct) =>
                await tourRepo.SingleOrDefaultAsync(new TourByNameSpec(name), ct)
                    is not Tour existingTour || existingTour.Id == tour.Id)
            .WithMessage((_, name) => string.Format(localizer["tour.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("A Description is required.");

        RuleFor(p => p.MinCapacity).
            NotEmpty()
            .WithMessage("A Minimum Capacity is required.");

        RuleFor(t => t.SelectedPickupLocations).NotEmpty().WithMessage("Please select a Pick Up Location.");
        RuleFor(t => t.SelectedDestinations).NotEmpty().WithMessage("Please select a Location.");
        
        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}