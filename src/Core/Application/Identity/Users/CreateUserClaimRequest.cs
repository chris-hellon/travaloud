namespace Travaloud.Application.Identity.Users;

public class CreateUserClaimRequest
{
    public string UserId { get; set; } = default!;
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set; } = default!;
}