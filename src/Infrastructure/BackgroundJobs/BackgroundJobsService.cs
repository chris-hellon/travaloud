using MediatR;
using Travaloud.Application.BackgroundJobs;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Infrastructure.Catalog.Services;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class BackgroundJobsService : BaseService, IBackgroundJobsService
{
    public BackgroundJobsService(ISender mediator) : base(mediator)
    {
    }
    
    public Task ImportCloudbedsGuests(ImportCloudbedsGuestsRequest request)
    {
        return Mediator.Send(request);
    }

    public Task SendDailyTourManifest(SendDailyTourManifestBatchRequest request)
    {
        return Mediator.Send(request);
    }
}