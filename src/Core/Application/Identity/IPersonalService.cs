using Travaloud.Application.Auditing;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Identity.Users.Password;

namespace Travaloud.Application.Identity;

public interface IPersonalService : ITransientService
{
    Task<UserDetailsDto?> GetProfileAsync(CancellationToken cancellationToken);

    Task UpdateProfileAsync(UpdateUserRequest request);

    Task ChangePasswordAsync(ChangePasswordRequest model);

    Task<List<string>> GetPermissionsAsync(CancellationToken cancellationToken);

    Task<List<AuditDto>> GetLogsAsync(DefaultIdType userId);
}