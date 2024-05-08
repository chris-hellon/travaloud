namespace Travaloud.Application.Cloudbeds.Dto;

public class GuestDto
{
    [JsonIgnore]
    public string Id { get; set; }
    
    [JsonProperty("guestFirstName")] public string FirstName { get; set; } = default!;
    [JsonProperty("guestLastName")] public string LastName { get; set; } = default!;
    [JsonProperty("guestGender")] public string Gender { get; set; } = default!;
    [JsonProperty("guestPhone")] public string Phone { get; set; } = default!;
    [JsonProperty("guestEmail")] public string Email { get; set; } = default!;
    [JsonProperty("guestCountry")] public string Nationality { get; set; } = default!;
    [JsonProperty("guestBirthDate")] public DateTime? DateOfBirth { get; set; } = default!;
    [JsonProperty("guestID")] public string GuestId { get; set; } = default!;
    [JsonProperty("customFields")] public CustomField[]? CustomFields { get; set; }
    [JsonProperty("guestDocumentType")] public string? GuestDocumentType { get; set; }
    [JsonProperty("guestDocumentNumber")] public string? GuestDocumentNumber { get; set; }
    [JsonProperty("guestDocumentIssueDate")] public string? GuestDocumentIssueDate { get; set; }
    [JsonProperty("guestDocumentIssuingCountry")] public string? GuestDocumentIssuingCountry { get; set; }
    [JsonProperty("guestDocumentExpirationDate")] public string? GuestDocumentExpirationDate { get; set; }
    
    [JsonIgnore] public string? HashedPassword { get; set; }

    [JsonIgnore]
    public string? PassportNumber { get; set; }
}

public class CustomField
{
    public string CustomFieldName { get; set; }
    public string CustomFieldValue { get; set; }
}
