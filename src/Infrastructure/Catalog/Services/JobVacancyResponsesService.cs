using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.JobVacanciesResponses.Commands;
using Travaloud.Application.Catalog.JobVacanciesResponses.Dto;
using Travaloud.Application.Catalog.JobVacanciesResponses.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class JobVacancyResponsesService: BaseService, IJobVacancyResponsesService
{
    public JobVacancyResponsesService(ISender mediator) : base(mediator)
    {
    }

    public Task<PaginationResponse<JobVacancyResponseDto>> SearchAsync(SearchJobVacancyResponsesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<JobVacancyResponseDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetJobVacancyResponseRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateJobVacancyResponseRequest request)
    {
        return Mediator.Send(request);
    }
}