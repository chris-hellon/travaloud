using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.EventHandlers;

public class PageCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Page>>
{
    private readonly ILogger<PageCreatedEventHandler> _logger;

    public PageCreatedEventHandler(ILogger<PageCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<Page> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}