using Travaloud.Application.Catalog.JobVacancies.Commands;
using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IJobVacanciesService : ITransientService
{
    Task<PaginationResponse<JobVacancyDto>> SearchAsync(SearchJobVacanciesRequest request);

    Task<JobVacancyDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreateJobVacancyRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateJobVacancyRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}