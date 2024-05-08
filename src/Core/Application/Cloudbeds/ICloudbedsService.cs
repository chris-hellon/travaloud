using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.Cloudbeds.Dto;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds;

public interface ICloudbedsService : ITransientService
{
    Task<GetPropertyAvailabilityResponse> GetPropertyAvailability(GetPropertyAvailabilityRequest request);

    Task<CreateReservationResponse> CreateReservation(CreateReservationRequest request);

    Task<CancelReservationResponse> CancelReservation(CancelReservationRequest request);
    Task<CreateReservationAdditionalGuestResponse> CreateReservationAdditionalGuest(CreateReservationAdditionalGuestRequest request);

    Task<CreateReservationPaymentResponse> CreateReservationPayment(CreateReservationPaymentRequest request);
    
    Task<GetGuestsResponse> GetGuests(GetGuestsRequest request);

    Task<IEnumerable<GuestDto>> SearchGuests(SearchCloudbedsGuests request);
}