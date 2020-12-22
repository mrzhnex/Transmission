using Core.Events;
namespace Core.Handlers
{
    public interface IEventHandlerDisconnect : IEventHandler
    {
        void OnDisconnect(DisconnectEvent disconnectEvent);
    }
}