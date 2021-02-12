using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerShouldLogChanged : IEventHandler
    {
        void OnShouldLogChanged(ShouldLogChangedEvent shouldLogChangedEvent);
    }
}