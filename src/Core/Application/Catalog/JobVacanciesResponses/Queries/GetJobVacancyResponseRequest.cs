using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Specification;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Queries;

public class GetJobVacancyResponseRequest : IRequest<JobVacancyResponseDto>
{
    public GetJobVacancyResponseRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetJobVacancyResponseRequestHandler : IRequestHandler<GetJobVacancyResponseRequest, JobVacancyResponseDto>
{
    private readonly IRepositoryFactory<JobVacancyResponse> _repository;
    private readonly IStringLocalizer<GetJobVacancyResponseRequestHandler> _localizer;

    public GetJobVacancyResponseRequestHandler(IRepositoryFactory<JobVacancyResponse> repository,
        IStringLocalizer<GetJobVacancyResponseRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<JobVacancyResponseDto> Handle(GetJobVacancyResponseRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new JobVacancyResponseByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["jobvacancyresponse.notfound"], request.Id));
}