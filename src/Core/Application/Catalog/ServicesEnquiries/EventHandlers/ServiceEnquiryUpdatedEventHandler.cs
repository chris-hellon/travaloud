using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.ServicesEnquiries.EventHandlers;

public class ServiceEnquiryUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<ServiceEnquiry>>
{
    private readonly ILogger<ServiceEnquiryUpdatedEventHandler> _logger;

    public ServiceEnquiryUpdatedEventHandler(ILogger<ServiceEnquiryUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<ServiceEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}