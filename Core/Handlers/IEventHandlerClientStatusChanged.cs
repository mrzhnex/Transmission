using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientStatusChanged : IEventHandler
    {
        void OnClientStatusChanged(ClientStatusChangedEvent clientStatusChangedEvent);
    }
}