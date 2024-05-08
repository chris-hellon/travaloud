using MediatR;
using Travaloud.Application.BackgroundJobs.Commands;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Commands;
using Travaloud.Application.Cloudbeds.Dto;
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

    public Task<CancelReservationResponse> CancelReservation(CancelReservationRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<CreateReservationAdditionalGuestResponse> CreateReservationAdditionalGuest(CreateReservationAdditionalGuestRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<CreateReservationPaymentResponse> CreateReservationPayment(CreateReservationPaymentRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<IEnumerable<GuestDto>> SearchGuests(SearchCloudbedsGuests request)
    {
        return Mediator.Send(request);
    }


    public async Task<GetGuestsResponse> GetGuests(GetGuestsRequest request)
    {
        var allGuests = new List<GuestDto>();
        var currentPage = 1;
        var pageSize = 100; // Adjust as per your API's pagination settings

        while (true)
        {
            // Set the page number in the request
            request.PageNumber = currentPage;
            request.PageSize = pageSize;

            // Call the mediator to get the response
            var response = await Mediator.Send(request);

            // Add the guests from the current page to the list
            if (response is {Success: true, Data: not null})
                allGuests.AddRange(response.Data);
            
            // If the total count of guests retrieved so far is equal to or exceeds the total count available, break the loop
            if (allGuests.Count >= response.Total || response.Data == null || !response.Data.Any())
                break;

            // Increment the page number for the next iteration
            currentPage++;
        }

        // Create and return the final response with all guests
        return new GetGuestsResponse
        {
            Count = allGuests.Count,
            Total = allGuests.Count,
            Data = allGuests,
            Success = true
        };
    }
}