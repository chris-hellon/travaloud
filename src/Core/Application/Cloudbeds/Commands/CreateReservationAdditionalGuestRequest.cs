using System.Net.Http.Headers;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationAdditionalGuestRequest : IRequest<CreateReservationAdditionalGuestResponse>
{
    [JsonProperty("propertyID")] public int? PropertyId { get; set; }

    [JsonProperty("reservationID")] public string? ReservationId { get; set; } 

    [JsonProperty("guestFirstName")] public string? GuestFirstName { get; set; } 
    
    [JsonProperty("guestLastName")] public string? GuestLastName { get; set; }

    [JsonProperty("guestGender")] public string? GuestGender { get; set; }

    [JsonProperty("guestEmail")] public string? GuestEmail { get; set; } 

    [JsonProperty("guestPhone")] public string? GuestPhone { get; set; } 

    [JsonProperty("guestBirthDate")] public DateTime? GuestBirthDate { get; set; }
    
    [JsonIgnore] public string? PropertyApiKey { get; set; }

    public CreateReservationAdditionalGuestRequest(
        int? propertyId,
        string? reservationId,
        string? guestFirstName,
        string? guestLastName,
        string? guestGender,
        string? guestEmail,
        string? guestPhone,
        DateTime? guestBirthDate,
        string? propertyApiKey)
    {
        PropertyId = propertyId;
        ReservationId = reservationId;
        GuestFirstName = guestFirstName;
        GuestLastName = guestLastName;
        GuestGender = guestGender;
        GuestEmail = guestEmail;
        GuestPhone = guestPhone;
        GuestBirthDate = guestBirthDate;
        PropertyApiKey = propertyApiKey;
    }
}

internal class CreateReservationAdditionalGuestRequestHandler : IRequestHandler<CreateReservationAdditionalGuestRequest, CreateReservationAdditionalGuestResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public CreateReservationAdditionalGuestRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }
    
    public async Task<CreateReservationAdditionalGuestResponse> Handle(CreateReservationAdditionalGuestRequest request, CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "postGuest");
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
        var responseObject = JsonConvert.DeserializeObject<CreateReservationAdditionalGuestResponse>(responseDataJson);

        return responseObject;
    }
}