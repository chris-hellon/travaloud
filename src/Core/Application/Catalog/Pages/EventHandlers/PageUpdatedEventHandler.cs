using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.EventHandlers;

public class PageUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Page>>
{
    private readonly ILogger<PageUpdatedEventHandler> _logger;

    public PageUpdatedEventHandler(ILogger<PageUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<Page> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}