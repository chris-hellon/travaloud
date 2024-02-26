using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds;

public interface ICloudbedsService : ITransientService
{
    Task<GetPropertyAvailabilityResponse> GetPropertyAvailability(GetPropertyAvailabilityRequest request);
}