using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.EventHandlers;

public class PageDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Page>>
{
    private readonly ILogger<PageDeletedEventHandler> _logger;

    public PageDeletedEventHandler(ILogger<PageDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<Page> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}