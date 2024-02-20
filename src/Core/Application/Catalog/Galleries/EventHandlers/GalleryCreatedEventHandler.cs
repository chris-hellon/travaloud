using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.EventHandlers;

public class GalleryCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Gallery>>
{
    private readonly ILogger<GalleryCreatedEventHandler> _logger;

    public GalleryCreatedEventHandler(ILogger<GalleryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Gallery> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}