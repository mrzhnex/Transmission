using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerInputNotFound : IEventHandler
    {
        void OnInputNotFound(InputNotFoundEvent InputNotFoundEvent);
    }
}