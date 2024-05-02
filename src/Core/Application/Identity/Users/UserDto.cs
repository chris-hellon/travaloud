namespace Travaloud.Application.Identity.Users;

public class UserDto
{
    public DefaultIdType Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    public string? Email { get; set; }
}