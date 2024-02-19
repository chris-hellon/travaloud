using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacancies.EventHandlers;

public class JobVacancyUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<JobVacancy>>
{
    private readonly ILogger<JobVacancyUpdatedEventHandler> _logger;

    public JobVacancyUpdatedEventHandler(ILogger<JobVacancyUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityUpdatedEvent<JobVacancy> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}