using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacancies.Commands;

public class CreateJobVacancyRequest : IRequest<DefaultIdType>
{
    public string Location { get; set; } = default!;
    public string? JobTitle { get; set; }
    public string? Description { get; set; }
    public string? CallToAction { get; set; }
}

public class CreateJobVacancyRequestHandler : IRequestHandler<CreateJobVacancyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<JobVacancy> _repository;

    public CreateJobVacancyRequestHandler(IRepositoryFactory<JobVacancy> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateJobVacancyRequest request, CancellationToken cancellationToken)
    {
        var jobVacancy = new JobVacancy(request.Location, request.JobTitle, request.Description, request.CallToAction);

        jobVacancy.DomainEvents.Add(EntityCreatedEvent.WithEntity(jobVacancy));
        
        await _repository.AddAsync(jobVacancy, cancellationToken);

        return jobVacancy.Id;
    }
}