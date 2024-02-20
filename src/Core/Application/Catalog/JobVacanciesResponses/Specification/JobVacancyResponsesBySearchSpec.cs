using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Queries;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Specification;

public class JobVacancyResponsesBySearchSpec : EntitiesByPaginationFilterSpec<JobVacancyResponse, JobVacancyResponseDto>
{
    public JobVacancyResponsesBySearchSpec(SearchJobVacancyResponsesRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.JobVacancy.JobTitle != null && p.JobVacancy.JobTitle.Equals(request.JobTitle), request.JobTitle != null)
            .Where(p => p.FirstName == request.FirstName, request.FirstName != null)
            .Where(p => p.LastName == request.LastName, request.LastName != null)
            .Where(p => p.CreatedOn == request.CreatedOn, request.LastName != null);
}
