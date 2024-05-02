namespace Travaloud.Application.Identity.Users;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
    public string? Role { get; set; }
}