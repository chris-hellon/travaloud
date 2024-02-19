using Travaloud.Application.Catalog.Events.Specification;
using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.Commands;

public class UpdateEventRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? BackgroundColor { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public FileUploadRequest? Image { get; set; }
}

public class UpdateEventRequestHandler : IRequestHandler<UpdateEventRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Event> _repository;
    private readonly IFileStorageService _file;

    public UpdateEventRequestHandler(IRepositoryFactory<Event> repository, IFileStorageService file)
    {
        _repository = repository;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var @event = await _repository.SingleOrDefaultAsync(new EventByIdSpec(request.Id), cancellationToken);

        if (@event == null)
        {
            throw new NotFoundException($"Event with ID {request.Id} not found.");
        }

        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentImagePath = @event.ImagePath;
            if (!string.IsNullOrEmpty(currentImagePath))
            {
                // Code to remove the image from storage
                // Assuming the file storage service has a Remove method
                await _file.Remove(currentImagePath);
            }

            @event.ClearImagePath();
        }

        var eventImagePath = request.Image is not null
            ? await _file.UploadAsync<Event>(request.Image, FileType.Image, cancellationToken)
            : null;

        @event.Update(request.Name, request.Description, request.ShortDescription, eventImagePath,
            request.BackgroundColor, request.PropertyId);

        // Add Domain Events to be raised after the commit
        @event.DomainEvents.Add(EntityUpdatedEvent.WithEntity(@event));

        await _repository.UpdateAsync(@event, cancellationToken);

        return request.Id;
    }
}