using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.JobVacancies.Commands;
using Travaloud.Application.Catalog.JobVacancies.Dto;
using Travaloud.Application.Catalog.JobVacancies.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class JobVacanciesService : BaseService, IJobVacanciesService
{
    public JobVacanciesService(ISender mediator) : base(mediator)
    {
    }

    public Task<PaginationResponse<JobVacancyDto>> SearchAsync(SearchJobVacanciesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<JobVacancyDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetJobVacancyRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateJobVacancyRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateJobVacancyRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteJobVacancyRequest(id));
    }
}