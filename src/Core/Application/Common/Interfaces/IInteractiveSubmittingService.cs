namespace Travaloud.Application.Common.Interfaces;

public interface IInteractiveSubmittingService : IScopedService
{
    bool BusySubmitting {get;set;}
    Func<Task> OnBusySubmittingChanged {get;set;}
    Task ToggleBusySubmitting(bool busySubmitting);
}