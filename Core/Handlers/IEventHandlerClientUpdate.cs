using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientUpdate : IEventHandler
    {
        void OnClientUpdate(ClientUpdateEvent clientUpdateEvent);
    }
}