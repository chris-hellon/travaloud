using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.EventHandlers;

public class JobVacancyResponseCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<JobVacancyResponse>>
{
    private readonly ILogger<JobVacancyResponseCreatedEventHandler> _logger;

    public JobVacancyResponseCreatedEventHandler(ILogger<JobVacancyResponseCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<JobVacancyResponse> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}