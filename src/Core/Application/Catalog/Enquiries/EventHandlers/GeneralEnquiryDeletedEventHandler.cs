using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Enquiries.EventHandlers;

public class GeneralEnquiryDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<GeneralEnquiry>>
{
    private readonly ILogger<GeneralEnquiryDeletedEventHandler> _logger;

    public GeneralEnquiryDeletedEventHandler(ILogger<GeneralEnquiryDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<GeneralEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}