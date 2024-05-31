namespace Travaloud.Application.Identity.Users.Password;

public class DeleteUserClaimRequest
{
    public string UserId { get; set; } = default!;
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set; } = default!;
}