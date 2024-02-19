using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.EventHandlers;

public class PageModalDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<PageModal>>
{
    private readonly ILogger<PageModalDeletedEventHandler> _logger;

    public PageModalDeletedEventHandler(ILogger<PageModalDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<PageModal> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}