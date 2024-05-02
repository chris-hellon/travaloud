namespace Travaloud.Application.Identity.Users;

public class CreateUserRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? ZipPostalCode { get; set; }
    public string? State { get; set; }
    public string? PassportNumber { get; set; }
    public DateTime? PassportIssueDate { get; set; }
    public DateTime? PassportExpiryDate { get; set; }
    public string? PassportIssuingCountry { get; set; }
    public DateTime? VisaIssueDate { get; set; }
    public DateTime? VisaExpiryDate { get; set; }
    public string? Origin { get; set; } = null;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public bool IsGuest { get; set; }
    public bool EmailRequired { get; set; } = true;
}