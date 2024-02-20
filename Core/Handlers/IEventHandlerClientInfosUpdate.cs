using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientInfosUpdate : IEventHandler
    {
        void OnClientInfosUpdate(ClientInfosUpdateEvent clientInfosUpdateEvent);
    }
}