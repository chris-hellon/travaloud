using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Queries;

public class GetReservationRequest: IRequest<CloudbedsResponse<GetReservationResponse>>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; }
    
    [JsonProperty("reservationID")] public string ReservationId { get; set; }

    [JsonIgnore] public string PropertyApiKey { get; set; } = default!;

    public GetReservationRequest(string propertyId, string reservationId, string propertyApiKey)
    {
        PropertyId = propertyId;
        ReservationId = reservationId;
        PropertyApiKey = propertyApiKey;
    }
}

internal class GetReservationRequestHandler : IRequestHandler<GetReservationRequest, CloudbedsResponse<GetReservationResponse>>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public GetReservationRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<CloudbedsResponse<GetReservationResponse>> Handle(GetReservationRequest request,
        CancellationToken cancellationToken)
    {
        var requestUri = _cloudbedsHttpClient.BuildApiUrl("getReservation", request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into GetPropertyAvailabilityResponse object
        var responseObject = JsonConvert.DeserializeObject<CloudbedsResponse<GetReservationResponse>>(responseDataJson);

        return responseObject;
    }
}