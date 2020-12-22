using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerServerUpdate : IEventHandler
    {
        void OnServerUpdate(ServerUpdateEvent serverUpdateEvent);
    }
}