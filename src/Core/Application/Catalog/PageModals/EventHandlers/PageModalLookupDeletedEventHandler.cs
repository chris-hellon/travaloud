using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.EventHandlers;

public class PageModalLookupDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<PageModalLookup>>
{
    private readonly ILogger<PageModalLookupDeletedEventHandler> _logger;

    public PageModalLookupDeletedEventHandler(ILogger<PageModalLookupDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<PageModalLookup> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}