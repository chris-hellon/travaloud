namespace Travaloud.Application.BackgroundJobs.Commands;

public class ImportCloudbedsGuestsRequest : IRequest<string>
{
    
}

public class ImportCloudbedsGuests : IRequest
{
    
}

internal class ImportCloudbedsGuestsRequestHandler : IRequestHandler<ImportCloudbedsGuestsRequest, string>
{
    private readonly IJobService _jobService;

    public ImportCloudbedsGuestsRequestHandler(IJobService jobService)
    {
        _jobService = jobService;
    }

    public Task<string> Handle(ImportCloudbedsGuestsRequest request, CancellationToken cancellationToken)
    {
        var jobId = $"Cloudbeds Guest Import";
        
        _jobService.AddOrUpdate(jobId, new ImportCloudbedsGuests(), "0 * * * *", TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        return Task.FromResult(jobId);
    }
}