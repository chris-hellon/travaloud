namespace Travaloud.Application.BackgroundJobs.Commands;

public class SendDailyTourManifestBatchRequest : IRequest
{
    
}

public class SendDailyTourManifestBatch : IRequest
{

}

internal class SendDailyTourManifestBatchRequestHandler : IRequestHandler<SendDailyTourManifestBatchRequest>
{
    private readonly IJobService _jobService;
    public SendDailyTourManifestBatchRequestHandler(
        IJobService jobService)
    {
        _jobService = jobService;
    }

    public Task Handle(SendDailyTourManifestBatchRequest request, CancellationToken cancellationToken)
    {
        var jobId = "Tours Daily Manifest";
        _jobService.AddOrUpdate(jobId,
            new SendDailyTourManifestBatch(),
            "0 0 * * *",
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        return Task.CompletedTask;
    }
}