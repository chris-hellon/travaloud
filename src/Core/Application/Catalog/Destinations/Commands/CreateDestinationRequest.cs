using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.Commands;

public class CreateDestinationRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Directions { get; set; }
    public string? GoogleMapsKey { get; set; }
    public FileUploadRequest? Image { get; set; }
}

public class CreateDestinationRequestHandler : IRequestHandler<CreateDestinationRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IFileStorageService _file;

    public CreateDestinationRequestHandler(IRepositoryFactory<Destination> repository, IFileStorageService file)
    {
        _repository = repository;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(CreateDestinationRequest request, CancellationToken cancellationToken)
    {
        var destinationImagePath = await _file.UploadAsync<Destination>(request.Image, FileType.Image, cancellationToken);

        var destination = new Destination(request.Name, request.Description, request.ShortDescription, request.Directions, destinationImagePath, destinationImagePath, request.GoogleMapsKey);

        // Add Domain Events to be raised after the commit
        destination.DomainEvents.Add(EntityCreatedEvent.WithEntity(destination));

        await _repository.AddAsync(destination, cancellationToken);

        return destination.Id;
    }
}