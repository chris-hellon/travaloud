using MediatR;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;
using Travaloud.Infrastructure.Catalog.Services;

namespace Travaloud.Infrastructure.Cloudbeds;

public class CloudbedsService : BaseService, ICloudbedsService
{
    public CloudbedsService(ISender mediator) : base(mediator)
    {
    }
    
    public async Task<GetPropertyAvailabilityResponse> GetPropertyAvailability(GetPropertyAvailabilityRequest request)
    {
        return await Mediator.Send(request);
    }

    public Task<CreateReservationResponse> CreateReservation(CreateReservationRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<CreateReservationAdditionalGuestResponse> CreateReservationAdditionalGuest(CreateReservationAdditionalGuestRequest request)
    {
        return Mediator.Send(request);
    }
}