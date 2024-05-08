using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Destinations.Validation;

public class DeleteDestinationRequestValidator : CustomValidator<DeleteDestinationRequest>
{
    private readonly IRepositoryFactory<Property> _propertyDestinationsRepo;
    private readonly IRepositoryFactory<Tour> _tourDestinationsRepo;
    private readonly IStringLocalizer<UpdateDestinationRequestValidator> _localizer;

    public DeleteDestinationRequestValidator(
        IRepositoryFactory<Property> propertyDestinationsRepo,
        IRepositoryFactory<Tour> tourDestinationsRepo,
        IStringLocalizer<UpdateDestinationRequestValidator> localizer)
    {
        _propertyDestinationsRepo = propertyDestinationsRepo;
        _tourDestinationsRepo = tourDestinationsRepo;
        _localizer = localizer;

        // Define custom validation rule
        RuleFor(p => p)
            .MustAsync(RepositoriesAreEmpty)
            .WithMessage(localizer["repositories.mustbeempty"]);
    }
    
    private async Task<bool> RepositoriesAreEmpty(DeleteDestinationRequest request, 
        CancellationToken cancellationToken)
    {
        // Check if either repository has a value
        var propertyRepoNotEmpty = await _propertyDestinationsRepo.AnyAsync(new PropertiesByDestinationIdSpec(request.Id), cancellationToken);
        var tourRepoNotEmpty = await _tourDestinationsRepo.AnyAsync(new ToursByDestinationIdSpec(request.Id), cancellationToken);

        // Return true if both repositories are empty, otherwise false
        return !propertyRepoNotEmpty && !tourRepoNotEmpty;
    }
}