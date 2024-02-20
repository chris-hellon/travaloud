using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacanciesResponses.Commands;

public class CreateJobVacancyResponseRequest : IRequest<DefaultIdType>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string HowCanWeCollaborate { get; set; } = default!;
    public string EstimatedDates { get; set; } = default!;
    public string DestinationsVisited { get; set; } = default!;
    public string? Instagram { get; set; }
    public string? TikTok { get; set; }
    public string? YouTube { get; set; }
    public string? Portfolio { get; set; }
    public string? Equipment { get; set; }
    public DefaultIdType JobVacancyId { get; set; } = default!;
}

public class CreateJobVacancyResponseRequestHandler : IRequestHandler<CreateJobVacancyResponseRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<JobVacancyResponse> _repository;

    public CreateJobVacancyResponseRequestHandler(IRepositoryFactory<JobVacancyResponse> repository)
    {
        _repository = repository;
    }
    
    public async Task<DefaultIdType> Handle(CreateJobVacancyResponseRequest request, CancellationToken cancellationToken)
    {
        var jobVacancyResponse = new JobVacancyResponse(
            request.FirstName,
            request.LastName,
            request.Email,
            request.HowCanWeCollaborate,
            request.EstimatedDates,
            request.DestinationsVisited,
            request.Instagram,
            request.TikTok,
            request.YouTube,
            request.Portfolio,
            request.Equipment,
            request.JobVacancyId);
        
        jobVacancyResponse.DomainEvents.Add(EntityCreatedEvent.WithEntity(jobVacancyResponse));
        
        await _repository.AddAsync(jobVacancyResponse, cancellationToken);

        return jobVacancyResponse.Id;
    }
}