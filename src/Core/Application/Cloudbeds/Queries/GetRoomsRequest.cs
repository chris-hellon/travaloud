using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Queries;

public class GetRoomsRequest : IRequest<GetRoomsResponse>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; } = default!;

    [JsonProperty("pageNumber")] public int PageNumber { get; set; } = 1;

    [JsonProperty("pageSize")] public int PageSize { get; set; } = 99999;
    
    [JsonIgnore] public string PropertyApiKey { get; set; } = default!;
}

internal class GetRoomsRequestHandler : IRequestHandler<GetRoomsRequest, GetRoomsResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public GetRoomsRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<GetRoomsResponse> Handle(GetRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var requestUri = _cloudbedsHttpClient.BuildApiUrl("getRooms", request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into GetPropertyAvailabilityResponse object
        var responseObject = JsonConvert.DeserializeObject<GetRoomsResponse>(responseDataJson);

        return responseObject;
    }
}