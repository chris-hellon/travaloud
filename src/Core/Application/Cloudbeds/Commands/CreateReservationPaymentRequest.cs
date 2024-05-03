using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationPaymentRequest : IRequest<CreateReservationPaymentResponse>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; }
    
    [JsonProperty("reservationID")] public string ReservationId { get; set; }
    
    [JsonProperty("type")] public string Type {get;set; } = "DIRECTFROMSTRIPE";

    [JsonProperty("amount")] public decimal Amount { get; set; }
    
    [JsonIgnore] public string? PropertyApiKey { get; set; }

    public CreateReservationPaymentRequest(string propertyId, string reservationId, decimal amount, string? propertyApiKey)
    {
        PropertyId = propertyId;
        ReservationId = reservationId;
        Amount = amount;
        PropertyApiKey = propertyApiKey;
    }
}

internal class CreateReservationPaymentRequestHandler : IRequestHandler<CreateReservationPaymentRequest, CreateReservationPaymentResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public CreateReservationPaymentRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<CreateReservationPaymentResponse> Handle(CreateReservationPaymentRequest request, CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "postPayment");
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
        var responseObject = JsonConvert.DeserializeObject<CreateReservationPaymentResponse>(responseDataJson);

        return responseObject;
    }
}