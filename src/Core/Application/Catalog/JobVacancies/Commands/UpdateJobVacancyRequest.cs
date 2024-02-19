using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacancies.Commands;

public class UpdateJobVacancyRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Location { get; set; } = default!;
    public string? JobTitle { get; set; }
    public string? Description { get; set; }
    public string? CallToAction { get; set; }
}

public class UpdateJobVacancyRequestHandler : IRequestHandler<UpdateJobVacancyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<JobVacancy> _repository;

    public UpdateJobVacancyRequestHandler(IRepositoryFactory<JobVacancy> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateJobVacancyRequest request, CancellationToken cancellationToken)
    {
        var jobVacancy = await _repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"Job vacancy with ID {request.Id} not found.");
        jobVacancy.Update(request.Location, request.JobTitle, request.Description, request.CallToAction);

        await _repository.UpdateAsync(jobVacancy, cancellationToken);

        return jobVacancy.Id;
    }
}