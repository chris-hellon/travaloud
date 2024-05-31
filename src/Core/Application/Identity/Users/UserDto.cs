namespace Travaloud.Application.Identity.Users;

public class UserDto
{
    public DefaultIdType Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    public string? FullName { get; set; }
    
    public string? Email { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string? Gender { get; set; }

    public string? Nationality { get; set; }
}