using Travaloud.Application.BackgroundJobs.Commands;

namespace Travaloud.Application.BackgroundJobs;

public interface ITourManifestService : ITransientService
{
    Task ScheduleTourManifest(SendDailyTourManifest request, CancellationToken cancellationToken);
}