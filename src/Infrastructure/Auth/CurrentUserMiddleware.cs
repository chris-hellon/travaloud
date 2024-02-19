using Finbuckle.MultiTenant;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Auth;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUserInitializer _currentUserInitializer;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    
    public CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer, IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor)
    {
        _currentUserInitializer = currentUserInitializer;
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUserInitializer.SetCurrentUser(context.User);

        await next(context);
    }
}