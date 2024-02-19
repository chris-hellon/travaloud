using MediatR;
using Travaloud.Application.Auditing;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Identity;
using Travaloud.Application.Identity.Users;
using Travaloud.Application.Identity.Users.Password;
using Travaloud.Infrastructure.Catalog.Services;

namespace Travaloud.Infrastructure.Identity;

public class PersonalService :  BaseService, IPersonalService
{
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    private string UserId => _currentUser.GetUserId().ToString();
    
    public PersonalService(IUserService userService, ICurrentUser currentUser, ISender mediator) : base(mediator)
    {
        _userService = userService;
        _currentUser = currentUser;
    }
    
    public async Task<UserDetailsDto?> GetProfileAsync(CancellationToken cancellationToken)
    {
        return await _userService.GetAsync(UserId, cancellationToken);
    }
    
    public async Task UpdateProfileAsync(UpdateUserRequest request)
    {
         await _userService.UpdateAsync(request, UserId);
    }
    
    public async Task ChangePasswordAsync(ChangePasswordRequest model)
    {
        await _userService.ChangePasswordAsync(model, UserId);
    }
    
    public async Task<List<string>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        return await _userService.GetPermissionsAsync(UserId, cancellationToken);
    }
    
    public Task<List<AuditDto>> GetLogsAsync(DefaultIdType userId)
    {
        return Mediator.Send(new GetMyAuditLogsRequest(userId));
    }
}