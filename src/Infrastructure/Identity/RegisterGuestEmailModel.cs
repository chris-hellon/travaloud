namespace Travaloud.Infrastructure.Identity;

public class RegisterGuestEmailModel : RegisterUserEmailModel
{
    public string Password { get; set; } = default!;
}