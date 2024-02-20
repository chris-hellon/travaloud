using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Specification;

public class JobVacancyResponseByIdSpec : Specification<JobVacancyResponse, JobVacancyResponseDto>, ISingleResultSpecification<JobVacancyResponse>
{
    public JobVacancyResponseByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}