using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Destinations.Specification;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Validation;

public class CreateDestinationRequestValidator : CustomValidator<CreateDestinationRequest>
{
    public CreateDestinationRequestValidator(IRepositoryFactory<Destination> destinationRepo, IStringLocalizer<CreateDestinationRequest> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(1024)
            .MustAsync(async (name, ct) => await destinationRepo.SingleOrDefaultAsync(new DestinationByNameSpec(name), ct) is null)
            .WithMessage((_, name) => string.Format(localizer["destination.alreadyexists"], name));

        RuleFor(p => p.Description)
            .NotEmpty();

        RuleFor(p => p.Image)
            .SetNonNullableValidator(new FileUploadRequestValidator());
    }
}