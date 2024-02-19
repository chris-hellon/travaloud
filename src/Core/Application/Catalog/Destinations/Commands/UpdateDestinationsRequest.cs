using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Destinations.Commands;

public class UpdateDestinationRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Directions { get; set; }
    public string? GoogleMapsKey { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
    public FileUploadRequest? Image { get; set; }
}

public class UpdateDestinationRequestHandler : IRequestHandler<UpdateDestinationRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IStringLocalizer<UpdateDestinationRequestHandler> _localizer;
    private readonly IFileStorageService _file;

    public UpdateDestinationRequestHandler(IRepositoryFactory<Destination> repository,
        IStringLocalizer<UpdateDestinationRequestHandler> localizer,
        IFileStorageService file)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(UpdateDestinationRequest request, CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = destination ?? throw new NotFoundException(string.Format(_localizer["destination.notfound"], request.Id));

        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = destination.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            destination = destination.ClearImagePath();
        }

        var destinationImagePath = request.Image is not null
            ? await _file.UploadAsync<Destination>(request.Image, FileType.Image, cancellationToken)
            : null;

        var updatedDestination = destination.Update(request.Name, request.Description, request.ShortDescription, request.Directions, destinationImagePath, destinationImagePath, request.GoogleMapsKey);

        // Add Domain Events to be raised after the commit
        destination.DomainEvents.Add(EntityUpdatedEvent.WithEntity(destination));

        await _repository.UpdateAsync(updatedDestination, cancellationToken);

        return request.Id;
    }
}