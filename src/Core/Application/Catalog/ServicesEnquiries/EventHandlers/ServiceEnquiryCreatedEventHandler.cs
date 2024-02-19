using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.ServicesEnquiries.EventHandlers;

public class ServiceEnquiryCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<ServiceEnquiry>>
{
    private readonly ILogger<ServiceEnquiryCreatedEventHandler> _logger;

    public ServiceEnquiryCreatedEventHandler(ILogger<ServiceEnquiryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<ServiceEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}