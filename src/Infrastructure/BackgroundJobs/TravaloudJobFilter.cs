using Hangfire.Client;
using Hangfire.Logging;
using Microsoft.Extensions.DependencyInjection;
using Travaloud.Infrastructure.Common;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Shared.Authorization;
using Travaloud.Shared.Multitenancy;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class TravaloudJobFilter : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

    private readonly IServiceProvider _services;

    public TravaloudJobFilter(IServiceProvider services) => _services = services;

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        Logger.InfoFormat("Set TenantId and UserId parameters to job {0}.{1}...", context.Job.Method.ReflectedType?.FullName, context.Job.Method.Name);

        using var scope = _services.CreateScope();

        var currentTenant = scope.ServiceProvider.GetService<TravaloudTenantInfo>();
        if (currentTenant?.Id is not null)
        {
            context.SetJobParameter(MultitenancyConstants.TenantIdName, currentTenant.Id);
        }

        var currentUser = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.User;
        if (currentUser?.GetUserId() is { } userId)
        {
            context.SetJobParameter(QueryStringKeys.UserId, userId);
        }
    }

    public void OnCreated(CreatedContext context) =>
        Logger.InfoFormat(
            "Job created with parameters {0}",
            context.Parameters.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2));
}