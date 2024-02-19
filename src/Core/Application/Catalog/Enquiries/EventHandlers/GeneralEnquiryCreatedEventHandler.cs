using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Enquiries.EventHandlers;

public class GeneralEnquiryCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<GeneralEnquiry>>
{
    private readonly ILogger<GeneralEnquiryCreatedEventHandler> _logger;

    public GeneralEnquiryCreatedEventHandler(ILogger<GeneralEnquiryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<GeneralEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}