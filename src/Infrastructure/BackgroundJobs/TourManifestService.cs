using MediatR;
using Travaloud.Application.BackgroundJobs;
using Travaloud.Application.BackgroundJobs.Commands;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class TourManifestService : ITourManifestService
{
    private readonly ISender _mediator;

    public TourManifestService(ISender mediator)
    {
        _mediator = mediator;
    }


    public Task ScheduleTourManifest(SendDailyTourManifest request, CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }
}