using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.EventHandlers;

public class GalleryUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Gallery>>
{
    private readonly ILogger<GalleryUpdatedEventHandler> _logger;

    public GalleryUpdatedEventHandler(ILogger<GalleryUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Gallery> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}