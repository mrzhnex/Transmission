using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerInputFound : IEventHandler
    {
        void OnInputFound(InputFoundEvent inputFoundEvent);
    }
}