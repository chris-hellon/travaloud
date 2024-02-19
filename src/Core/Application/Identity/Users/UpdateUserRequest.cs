namespace Travaloud.Application.Identity.Users;

public class UpdateUserRequest
{
    public string Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; } = false;
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
    public bool IsGuest { get; set; }
    public IEnumerable<string>? Genders { get; set; }
}