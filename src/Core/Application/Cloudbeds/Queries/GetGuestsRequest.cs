using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Queries;

public class GetGuestsRequest: IRequest<GetGuestsResponse>
{
    [JsonProperty("pageNumber")] public int PageNumber { get; set; } = 1;

    [JsonProperty("pageSize")] public int PageSize { get; set; } = 100;
    
    [JsonProperty("propertyID")] public string PropertyId { get; set; }

    [JsonProperty("includeGuestInfo")] public bool IncludeGuestInfo { get; set; } = true;
    
    [JsonProperty("checkInFrom")] public string? ResultsFrom { get; set; }

    [JsonProperty("checkInTo")] public string? ResultsTo { get; set; }

    [JsonProperty("excludeSecondaryGuests")]
    public bool ExcludeSecondaryGuests { get; set; } = true;

    [JsonIgnore] public string PropertyApiKey { get; set; } = default!;

    public GetGuestsRequest(string propertyId, string propertyApiKey)
    {
        PropertyId = propertyId;
        PropertyApiKey = propertyApiKey;
    }
}

internal class GetGuestsRequestHandler : IRequestHandler<GetGuestsRequest, GetGuestsResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public GetGuestsRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<GetGuestsResponse> Handle(GetGuestsRequest request,
        CancellationToken cancellationToken)
    {
        var requestUri = _cloudbedsHttpClient.BuildApiUrl("getGuestList", request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into GetPropertyAvailabilityResponse object
        var responseObject = JsonConvert.DeserializeObject<GetGuestsResponse>(responseDataJson);

        return responseObject;
    }
}