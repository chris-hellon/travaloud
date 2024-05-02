using System.Linq.Expressions;
using System.Text.Json;
using Hangfire;
using MediatR;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.BackgroundJobs;

public class HangfireService : IJobService
{
    private readonly TravaloudTenantInfo _currentTenant;
    private readonly ICurrentUser _currentUser;
    
    public HangfireService(TravaloudTenantInfo currentTenant, ICurrentUser currentUser) =>
        (_currentTenant, _currentUser) = (currentTenant, currentUser);
    
    public bool Delete(string jobId) =>
        BackgroundJob.Delete(jobId);

    public bool Delete(string jobId, string fromState) =>
        BackgroundJob.Delete(jobId, fromState);

    public string Enqueue(IRequest request) =>
        BackgroundJob.Enqueue<HangfireMediatorBridge>(bridge => bridge.Send(GetDisplayName(request), request, default));
    
    public void AddOrUpdate(string recurringJobId, IRequest request, string cronExpression, TimeZoneInfo? timeZone = null, string queue = "default") =>
        RecurringJob.AddOrUpdate<HangfireMediatorBridge>(
            recurringJobId,
            bridge => bridge.Send(GetDisplayName(request), _currentTenant.Id, _currentUser.GetUserId().ToString(), request, default),
            cronExpression,
            timeZone,
            queue);
    
    public string Enqueue(Expression<Func<Task>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Action<T>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue(Expression<Action> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall) =>
        BackgroundJob.Enqueue(methodCall);

    public bool Requeue(string jobId) =>
        BackgroundJob.Requeue(jobId);

    public bool Requeue(string jobId, string fromState) =>
        BackgroundJob.Requeue(jobId, fromState);

    public string Schedule(Expression<Action> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) =>
        BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt) =>
        BackgroundJob.Schedule(methodCall, enqueueAt);
    
    private static string GetDisplayName(IRequest request) => $"{request.GetType().Name} {JsonSerializer.Serialize(request, request.GetType())}";
}