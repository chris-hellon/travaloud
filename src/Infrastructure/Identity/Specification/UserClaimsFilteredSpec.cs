using Ardalis.Specification;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Travaloud.Application.Common.Specification;
using Travaloud.Application.Identity.Users;

namespace Travaloud.Infrastructure.Identity.Specification;

public class UserClaimsFilteredSpec : EntitiesByBaseFilterSpec<IdentityUserClaim<string>>
{
    public UserClaimsFilteredSpec(GetUserClaimsRequest request) : base(request) =>
        Query.Where(p => p.UserId == request.UserId)
            .Where(x => x.ClaimType == request.ClaimType, condition: !request.ClaimType.IsNullOrEmpty())
            .Where(x => x.ClaimValue == request.ClaimValue, condition: !request.ClaimValue.IsNullOrEmpty());
}