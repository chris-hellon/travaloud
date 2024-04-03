using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds;

public interface ICloudbedsService : ITransientService
{
    Task<GetPropertyAvailabilityResponse> GetPropertyAvailability(GetPropertyAvailabilityRequest request);

    Task<CreateReservationResponse> CreateReservation(CreateReservationRequest request);

    Task<CreateReservationAdditionalGuestResponse> CreateReservationAdditionalGuest(CreateReservationAdditionalGuestRequest request);
}