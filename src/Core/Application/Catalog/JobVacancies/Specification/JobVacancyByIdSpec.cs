using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Domain.Catalog.JobVacancies;

namespace Travaloud.Application.Catalog.JobVacancies.Specification;

public class JobVacancyByIdSpec : Specification<JobVacancy, JobVacancyDto>, ISingleResultSpecification<JobVacancy>
{
    public JobVacancyByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}