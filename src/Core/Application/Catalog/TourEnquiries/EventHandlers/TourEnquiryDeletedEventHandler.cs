using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TourEnquiries.EventHandlers;

public class TourEnquiryDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<TourEnquiry>>
{
    private readonly ILogger<TourEnquiryDeletedEventHandler> _logger;

    public TourEnquiryDeletedEventHandler(ILogger<TourEnquiryDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<TourEnquiry> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}