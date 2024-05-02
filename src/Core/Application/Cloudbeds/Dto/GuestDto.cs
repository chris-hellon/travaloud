namespace Travaloud.Application.Cloudbeds.Dto;

public class GuestDto
{
    [JsonProperty("guestFirstName")] public string FirstName { get; set; } = default!;
    [JsonProperty("guestLastName")] public string LastName { get; set; } = default!;
    [JsonProperty("guestGender")] public string Gender { get; set; } = default!;
    [JsonProperty("guestPhone")] public string Phone { get; set; } = default!;
    [JsonProperty("guestEmail")] public string Email { get; set; } = default!;
    [JsonProperty("guestCountry")] public string Nationality { get; set; } = default!;
    [JsonProperty("guestBirthDate")] public DateTime? DateOfBirth { get; set; } = default!;
    [JsonIgnore] public string? HashedPassword { get; set; }
}