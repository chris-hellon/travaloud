using Microsoft.AspNetCore.Identity;

namespace Travaloud.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? ObjectId { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool EmailSubscribed { get; set; } = true;
    public DateTime? SignUpDate { get; set; }
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
}