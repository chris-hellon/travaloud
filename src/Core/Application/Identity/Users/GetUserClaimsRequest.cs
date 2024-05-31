namespace Travaloud.Application.Identity.Users;

public class GetUserClaimsRequest : BaseFilter
{
    public string UserId { get; set; } = default!;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}