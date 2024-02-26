using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Queries;

public class GetPropertyAvailabilityRequest : IRequest<GetPropertyAvailabilityResponse>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; } = default!;

    [JsonProperty("startDate")] public string StartDate { get; set; } = default!;

    [JsonProperty("endDate")] public string EndDate { get; set; } = default!;

    [JsonIgnore] public string PropertyApiKey { get; set; } = default!;
}

public class
    GetPropertyAvailabilityRequestHandler : IRequestHandler<GetPropertyAvailabilityRequest,
    GetPropertyAvailabilityResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public GetPropertyAvailabilityRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<GetPropertyAvailabilityResponse> Handle(GetPropertyAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var requestUri = _cloudbedsHttpClient.BuildApiUrl("getAvailableRoomTypes", request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into GetPropertyAvailabilityResponse object
        var responseObject = JsonConvert.DeserializeObject<GetPropertyAvailabilityResponse>(responseDataJson);

        return responseObject;
    }
}