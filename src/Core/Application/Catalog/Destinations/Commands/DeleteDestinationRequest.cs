using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.Commands;

public class DeleteDestinationRequest : IRequest<DefaultIdType>
{
    public DeleteDestinationRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteDestinationRequestHandler : IRequestHandler<DeleteDestinationRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IStringLocalizer<DeleteDestinationRequestHandler> _localizer;
    private readonly IRepositoryFactory<Property> _propertyDestinationsRepo;
    private readonly IRepositoryFactory<Tour> _tourDestinationsRepo;

    public DeleteDestinationRequestHandler(IRepositoryFactory<Destination> repository,
        IStringLocalizer<DeleteDestinationRequestHandler> localizer, 
        IRepositoryFactory<Tour> tourDestinationsRepo,
        IRepositoryFactory<Property> propertyDestinationsRepo)
    {
        _repository = repository;
        _localizer = localizer;
        _tourDestinationsRepo = tourDestinationsRepo;
        _propertyDestinationsRepo = propertyDestinationsRepo;
    }

    public async Task<DefaultIdType> Handle(DeleteDestinationRequest request, CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = destination ?? throw new NotFoundException(_localizer["destination.notfound"]);

        var propertyRepoNotEmptyTask = Task.Run(() => _propertyDestinationsRepo.AnyAsync(new PropertiesByDestinationIdSpec(request.Id), cancellationToken), cancellationToken);
        var tourRepoNotEmptyTask = Task.Run(() => _tourDestinationsRepo.AnyAsync(new ToursByDestinationIdSpec(request.Id), cancellationToken), cancellationToken);

        await Task.WhenAll(propertyRepoNotEmptyTask, tourRepoNotEmptyTask);
        
        if (propertyRepoNotEmptyTask.Result || tourRepoNotEmptyTask.Result)
            throw new CustomException("Unable to delete this Destination, as there are Tours or Properties associated with it.");
        
        
        // Add Domain Events to be raised after the commit
        destination.DomainEvents.Add(EntityDeletedEvent.WithEntity(destination));

        await _repository.DeleteAsync(destination, cancellationToken);

        return request.Id;
    }
}