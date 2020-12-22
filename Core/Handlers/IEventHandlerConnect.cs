using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerConnect : IEventHandler
    {
        void OnConnect(ConnectEvent connectEvent);
    }
}