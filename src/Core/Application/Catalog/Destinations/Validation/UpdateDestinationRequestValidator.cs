using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Validation;

public class UpdateDestinationRequestValidator : CustomValidator<UpdateDestinationRequest>
{
    public UpdateDestinationRequestValidator(IRepositoryFactory<Destination> destinationRepo, IStringLocalizer<UpdateDestinationRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (destination, name, ct) =>
                await destinationRepo.FirstOrDefaultAsync(new DestinationByNameSpec(name), ct)
                    is not Destination existingDestination || existingDestination.Id == destination.Id)
            .WithMessage((_, name) => string.Format(localizer["destination.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}