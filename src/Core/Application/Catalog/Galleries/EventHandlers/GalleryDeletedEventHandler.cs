using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.EventHandlers;

public class GalleryDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Gallery>>
{
    private readonly ILogger<GalleryDeletedEventHandler> _logger;

    public GalleryDeletedEventHandler(ILogger<GalleryDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Gallery> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}