using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Specification;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacancies.Queries;

public class GetJobVacancyRequest : IRequest<JobVacancyDto>
{
    public GetJobVacancyRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetJobVacancyRequestHandler : IRequestHandler<GetJobVacancyRequest, JobVacancyDto>
{
    private readonly IRepositoryFactory<JobVacancy> _repository;
    private readonly IStringLocalizer<GetJobVacancyRequestHandler> _localizer;

    public GetJobVacancyRequestHandler(IRepositoryFactory<JobVacancy> repository,
        IStringLocalizer<GetJobVacancyRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<JobVacancyDto> Handle(GetJobVacancyRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new JobVacancyByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["jobvacancy.notfound"], request.Id));
}