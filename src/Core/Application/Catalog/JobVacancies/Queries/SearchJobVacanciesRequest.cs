using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Specification;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacancies.Queries;

public class SearchJobVacanciesRequest : PaginationFilter, IRequest<PaginationResponse<JobVacancyDto>>
{
    public string? JobTitle { get; set; }
}

public class SearchEventsRequestHandler : IRequestHandler<SearchJobVacanciesRequest, PaginationResponse<JobVacancyDto>>
{
    private readonly IRepositoryFactory<JobVacancy> _repository;

    public SearchEventsRequestHandler(IRepositoryFactory<JobVacancy> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<JobVacancyDto>> Handle(SearchJobVacanciesRequest request, CancellationToken cancellationToken)
    {
        var spec = new JobVacanciesBySearchSpc(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}