using Travaloud.Application.Catalog.JobVacanciesResponses.Commands;
using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IJobVacancyResponsesService : ITransientService
{
    Task<PaginationResponse<JobVacancyResponseDto>> SearchAsync(SearchJobVacancyResponsesRequest request);

    Task<JobVacancyResponseDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreateJobVacancyResponseRequest request);
}