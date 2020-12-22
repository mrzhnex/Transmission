using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerInput : IEventHandler
    {
        void OnInput(InputEvent inputEvent);
    }
}