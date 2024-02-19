using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Enquiries.EventHandlers;

public class GeneralEnquiryUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<GeneralEnquiry>>
{
    private readonly ILogger<GeneralEnquiryUpdatedEventHandler> _logger;

    public GeneralEnquiryUpdatedEventHandler(ILogger<GeneralEnquiryUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<GeneralEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}