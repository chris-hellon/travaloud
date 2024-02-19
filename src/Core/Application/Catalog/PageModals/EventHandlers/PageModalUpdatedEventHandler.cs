using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.EventHandlers;

public class PageModalUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<PageModal>>
{
    private readonly ILogger<PageModalUpdatedEventHandler> _logger;

    public PageModalUpdatedEventHandler(ILogger<PageModalUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<PageModal> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}