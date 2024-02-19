using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.EventHandlers;

public class PageModalCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<PageModal>>
{
    private readonly ILogger<PageModalCreatedEventHandler> _logger;

    public PageModalCreatedEventHandler(ILogger<PageModalCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<PageModal> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}