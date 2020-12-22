using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerShutdown : IEventHandler
    {
        void OnShutdown(ShutdownEvent shutdownEvent);
    }
}