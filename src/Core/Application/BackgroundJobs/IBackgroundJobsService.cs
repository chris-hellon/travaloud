using Travaloud.Application.BackgroundJobs.Commands;

namespace Travaloud.Application.BackgroundJobs;

public interface IBackgroundJobsService : ITransientService
{
    Task ImportCloudbedsGuests(ImportCloudbedsGuestsRequest request);

    Task SendDailyTourManifest(SendDailyTourManifestBatchRequest request);
}