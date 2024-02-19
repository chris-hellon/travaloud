using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TourEnquiries.EventHandlers;

public class TourEnquiryCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<TourEnquiry>>
{
    private readonly ILogger<TourEnquiryCreatedEventHandler> _logger;

    public TourEnquiryCreatedEventHandler(ILogger<TourEnquiryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<TourEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}