using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Queries;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacancies.Specification;

public class JobVacanciesBySearchSpc : EntitiesByPaginationFilterSpec<JobVacancy, JobVacancyDto>
{
    public JobVacanciesBySearchSpc(SearchJobVacanciesRequest request)
        : base(request) =>
        Query
            .OrderBy(c => c.JobTitle, !request.HasOrderBy())
            .Where(p => p.JobTitle != null && p.JobTitle.Equals(request.JobTitle), request.JobTitle != null);
}