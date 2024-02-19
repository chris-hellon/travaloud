using Travaloud.Application.Identity.Users.Password;

namespace Travaloud.Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);

    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);

    Task ChangePasswordAsync(ChangePasswordRequest model, string userId);
    Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken, string? role = null);
    Task<List<UserDetailsDto>> GetListAsync(string? role = null);

    Task<int> GetCountAsync(CancellationToken cancellationToken, string? role = null);

    Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken);
    Task<UserDetailsDto> GetAsync(string userId);
    
    Task<List<UserRoleDto>> GetRolesAsync(string userId);
    Task<string> AssignRolesAsync(string userId, UserRolesRequest request);

    Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken);

    Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken);
    Task ToggleStatusAsync(ToggleUserStatusRequest request);
    
    Task<string> CreateAsync(CreateUserRequest request, string origin, string roleName);
    Task UpdateAsync(UpdateUserRequest request, string userId);
}