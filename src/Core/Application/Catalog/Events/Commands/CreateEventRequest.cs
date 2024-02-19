using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Events.Commands;

public class CreateEventRequest : IRequest<DefaultIdType>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? BackgroundColor { get; set; }
    public DefaultIdType? PropertyId { get; set; }

    public FileUploadRequest? Image { get; set; }
}

public class CreateEventRequestHandler : IRequestHandler<CreateEventRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Event> _repository;
    private readonly IFileStorageService _file;

    public CreateEventRequestHandler(IRepositoryFactory<Event> repository, IFileStorageService file)
    {
        _repository = repository;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(CreateEventRequest request, CancellationToken cancellationToken)
    {
        var eventImagePath = await _file.UploadAsync<Event>(request.Image, FileType.Image, cancellationToken);

        var @event = new Event(request.Name!, request.Description, request.ShortDescription, eventImagePath,
            request.BackgroundColor, request.PropertyId);

        // Add Domain Events to be raised after the commit
        @event.DomainEvents.Add(EntityCreatedEvent.WithEntity(@event));

        await _repository.AddAsync(@event, cancellationToken);

        return @event.Id;
    }
}