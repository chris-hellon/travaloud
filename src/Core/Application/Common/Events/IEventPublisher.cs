using Travaloud.Shared.Events;

namespace Travaloud.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}