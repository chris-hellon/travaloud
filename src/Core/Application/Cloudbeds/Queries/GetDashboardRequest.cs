using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Queries;

public class GetDashboardRequest : IRequest<CloudbedsResponse<GetDashboardResponse>>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; }

    [JsonIgnore] public string PropertyApiKey { get; set; }

    public GetDashboardRequest(string propertyId, string propertyApiKey)
    {
        PropertyId = propertyId;
        PropertyApiKey = propertyApiKey;
    }
}

internal class GetDashboardRequestHandler : IRequestHandler<GetDashboardRequest, CloudbedsResponse<GetDashboardResponse>>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public GetDashboardRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<CloudbedsResponse<GetDashboardResponse>> Handle(GetDashboardRequest request,
        CancellationToken cancellationToken)
    {
        var requestUri = _cloudbedsHttpClient.BuildApiUrl("getDashboard", request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into GetPropertyAvailabilityResponse object
        var responseObject = JsonConvert.DeserializeObject<CloudbedsResponse<GetDashboardResponse>>(responseDataJson);

        return responseObject;
    }
}
