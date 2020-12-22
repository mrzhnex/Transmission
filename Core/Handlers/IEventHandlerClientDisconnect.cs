using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientDisconnect : IEventHandler
    {
        void OnClientDisconnect(ClientDisconnectEvent clientDisconnectEvent);
    }
}