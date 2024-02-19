using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TourEnquiries.EventHandlers;

public class TourEnquiryUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<TourEnquiry>>
{
    private readonly ILogger<TourEnquiryUpdatedEventHandler> _logger;

    public TourEnquiryUpdatedEventHandler(ILogger<TourEnquiryUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<TourEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}