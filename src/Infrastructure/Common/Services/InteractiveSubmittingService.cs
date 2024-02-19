using Travaloud.Application.Common.Interfaces;

namespace Travaloud.Infrastructure.Common.Services;

public class InteractiveSubmittingService : IInteractiveSubmittingService
{
    public bool BusySubmitting {get;set;}
    public Func<Task> OnBusySubmittingChanged { get; set; } = default!;
    public async Task ToggleBusySubmitting(bool busySubmitting)
    {
        BusySubmitting = busySubmitting;
        await OnBusySubmittingChanged.Invoke();
    }
}