using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.ServicesEnquiries.EventHandlers;

public class ServiceEnquiryDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<ServiceEnquiry>>
{
    private readonly ILogger<ServiceEnquiryDeletedEventHandler> _logger;

    public ServiceEnquiryDeletedEventHandler(ILogger<ServiceEnquiryDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<ServiceEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}