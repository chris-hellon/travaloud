using Ardalis.Specification;
using Travaloud.Application.Common.Specification;
using Travaloud.Application.Identity.Users;

namespace Travaloud.Infrastructure.Identity.Specification;

public class UsersByIdsSpec : EntitiesByBaseFilterSpec<ApplicationUser>
{
    public UsersByIdsSpec(UsersByIdsRequest request) : base(request) =>
        Query.Where(p => request.UserIds.Contains(p.Id));
}