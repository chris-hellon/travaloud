using System.Net.Http.Headers;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Cloudbeds.Responses;

namespace Travaloud.Application.Cloudbeds.Commands;

public class CreateReservationRequest : IRequest<CreateReservationResponse>
{
    [JsonProperty("propertyID")] public string PropertyId { get; set; }

    [JsonProperty("thirdPartyIdentifier")] public string ThirdPartyIdentifier { get; set; }

    [JsonProperty("startDate")] public string StartDate { get; set; }

    [JsonProperty("endDate")] public string EndDate { get; set; }

    [JsonProperty("guestFirstName")] public string GuestFirstName { get; set; }

    [JsonProperty("guestLastName")] public string GuestLastName { get; set; }

    [JsonProperty("guestGender")] public string GuestGender { get; set; }

    [JsonProperty("guestCountry")] public string GuestCountry { get; set; } = string.Empty;

    [JsonProperty("guestZip")] public string GuestZip { get; set; }

    [JsonProperty("guestEmail")] public string GuestEmail { get; set; }

    [JsonProperty("guestPhone")] public string GuestPhone { get; set; }

    [JsonProperty("estimatedArrivalTime")] public string EstimatedArrivalTime { get; set; }

    [JsonProperty("rooms")] public CreateReservationRoomRequest[] Rooms { get; set; }

    [JsonProperty("adults")] public CreateReservationGuestRequest[] Adults { get; set; }

    [JsonProperty("children")] public CreateReservationGuestRequest[] Children { get; set; }

    [JsonProperty("paymentMethod")] public string PaymentMethod { get; set; }

    [JsonProperty("cardToken")] public string CardToken { get; set; }

    [JsonProperty("paymentAuthorizationCode")] public string PaymentAuthorizationCode { get; set; }

    [JsonProperty("promoCode")] public string PromoCode { get; set; } = string.Empty;

    [JsonIgnore] public string? PropertyApiKey { get; set; }

    public CreateReservationRequest(
        BasketModel basket, 
        BasketItemModel basketItem, 
        string? propertyApiKey,
        string cardToken, 
        string paymentAuthorizationCode)
    {
        PropertyApiKey = propertyApiKey;
        PropertyId = basketItem.CloudbedsPropertyId?.ToString() ?? string.Empty;
        GuestFirstName = basket.FirstName ?? string.Empty;
        GuestLastName = basket.Surname ?? string.Empty;
        GuestGender = basket.Gender switch
        {
            "Male" => "M",
            "Female" => "F",
            _ => "N/A"
        };
        if (basket.Nationality != null) GuestCountry = basket.Nationality.CountryToTwoLetterCode() ?? string.Empty;
        GuestZip = "";
        GuestEmail = basket.Email ?? string.Empty;
        GuestPhone = basket.PhoneNumber ?? string.Empty;
        EstimatedArrivalTime = basket.EstimatedArrivalTime?.ToString("hh\\:mm") ?? string.Empty;
        var roomsRequests = new List<CreateReservationRoomRequest>();
        var adultsRequests = new List<CreateReservationGuestRequest>();
        var childrenRequests = new List<CreateReservationGuestRequest>();

        if (basketItem.Rooms != null && basketItem.Rooms.Any())
        {
            roomsRequests.AddRange(basketItem.Rooms.Select(room => new CreateReservationRoomRequest()
                {RoomTypeId = room.RoomTypeId.ToString(), Quantity = room.RoomQuantity}));

            var adults = basketItem.Rooms.Where(x => x.AdultQuantity > 0);
            var adultsModels = adults as BasketItemRoomModel[] ?? adults.ToArray();

            if (adultsModels.Length != 0)
            {
                adultsRequests.AddRange(adultsModels.Select(adult => new CreateReservationGuestRequest
                    {RoomTypeId = adult.RoomTypeId.ToString(), Quantity = adult.RoomQuantity}));
            }
            else
            {
                adultsRequests.AddRange(basketItem.Rooms.Select(adult => new CreateReservationGuestRequest
                    {RoomTypeId = adult.RoomTypeId.ToString(), Quantity = adult.RoomQuantity}));
            }

            var children = basketItem.Rooms.Where(x => x.ChildrenQuantity > 0);
            var childrenModels = children as BasketItemRoomModel[] ?? children.ToArray();

            if (childrenModels.Length != 0)
            {
                childrenRequests.AddRange(childrenModels.Select(child => new CreateReservationGuestRequest
                    {RoomTypeId = child.RoomTypeId.ToString(), Quantity = child.RoomQuantity}));
            }
            else
            {
                childrenRequests.AddRange(basketItem.Rooms.Select(adult => new CreateReservationGuestRequest
                    {RoomTypeId = adult.RoomTypeId.ToString(), Quantity = 0}));
            }
        }

        Rooms = roomsRequests.ToArray();
        Adults = adultsRequests.ToArray();
        Children = childrenRequests.ToArray();

        StartDate = basketItem.CheckInDateParsed?.ToString("yyyy-MM-dd") ?? string.Empty;
        EndDate = basketItem.CheckOutDateParsed?.ToString("yyyy-MM-dd") ?? string.Empty;

        PaymentMethod = "credit";
        CardToken = cardToken;
        PaymentAuthorizationCode = paymentAuthorizationCode;
        ThirdPartyIdentifier = "Fuse Website / Travaloud";
    }
}

internal class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, CreateReservationResponse>
{
    private readonly ICloudbedsHttpClient _cloudbedsHttpClient;

    public CreateReservationRequestHandler(ICloudbedsHttpClient cloudbedsHttpClient)
    {
        _cloudbedsHttpClient = cloudbedsHttpClient ?? throw new ArgumentNullException(nameof(cloudbedsHttpClient));
    }

    public async Task<CreateReservationResponse> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "postReservation");
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
        var responseObject = JsonConvert.DeserializeObject<CreateReservationResponse>(responseDataJson);

        return responseObject;
    }
}