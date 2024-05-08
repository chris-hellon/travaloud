using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Commands;

public class CancelReservationRequest : IRequest<CancelReservationResponse>
{
    [JsonProperty("reservationID")] public string ReservationId { get; set; }
    
    [JsonIgnore] [JsonProperty("propertyID")] public string PropertyId { get; set; }

    [JsonProperty("status")] public string Status => "canceled";
    
    [JsonIgnore] public string? PropertyApiKey { get; set; }

    public CancelReservationRequest(string reservationId, string? propertyApiKey, string propertyId)
    {
        ReservationId = reservationId;
        PropertyApiKey = propertyApiKey;
        PropertyId = propertyId;
    }
}

internal class CancelReservationRequestHandler : IRequestHandler<CancelReservationRequest, CancelReservationResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public CancelReservationRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<CancelReservationResponse> Handle(CancelReservationRequest request, CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"putReservation?propertyID={request.PropertyId}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.PropertyApiKey);

        //var requestString = JsonConvert.SerializeObject(request);
        var requestData = request.FlattenObject();
        
        using var formContent = new FormUrlEncodedContent(requestData);

        httpRequest.Content = formContent;
        
        // Send HTTP request
        var response = await _cloudbedsHttpClient.SendAsync(httpRequest, cancellationToken);

        // Ensure success status code
        response.EnsureSuccessStatusCode();

        // Deserialize response content
        var responseDataJson = await response.Content.ReadAsStringAsync(cancellationToken);

        // Deserialize JSON into CreateReservationResponse object
        var responseObject = JsonConvert.DeserializeObject<CancelReservationResponse>(responseDataJson);

        return responseObject;
    }
}