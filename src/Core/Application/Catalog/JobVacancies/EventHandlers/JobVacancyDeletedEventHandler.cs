using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacancies.EventHandlers;

public class JobVacancyDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<JobVacancy>>
{
    private readonly ILogger<JobVacancyDeletedEventHandler> _logger;

    public JobVacancyDeletedEventHandler(ILogger<JobVacancyDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityDeletedEvent<JobVacancy> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}