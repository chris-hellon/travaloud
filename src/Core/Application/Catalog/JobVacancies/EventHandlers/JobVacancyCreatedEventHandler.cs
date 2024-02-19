using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacancies.EventHandlers;

public class JobVacancyCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<JobVacancy>>
{
    private readonly ILogger<JobVacancyCreatedEventHandler> _logger;

    public JobVacancyCreatedEventHandler(ILogger<JobVacancyCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(EntityCreatedEvent<JobVacancy> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}