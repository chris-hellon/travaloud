using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Specification;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Queries;

public class SearchJobVacancyResponsesRequest : PaginationFilter, IRequest<PaginationResponse<JobVacancyResponseDto>>
{
    public string? JobTitle { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class SearchJobVacanyResponsesRequestHandler : IRequestHandler<SearchJobVacancyResponsesRequest, PaginationResponse<JobVacancyResponseDto>>
{
    private readonly IRepositoryFactory<JobVacancyResponse> _repository;

    public SearchJobVacanyResponsesRequestHandler(IRepositoryFactory<JobVacancyResponse> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<JobVacancyResponseDto>> Handle(SearchJobVacancyResponsesRequest request, CancellationToken cancellationToken)
    {
        var spec = new JobVacancyResponsesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}