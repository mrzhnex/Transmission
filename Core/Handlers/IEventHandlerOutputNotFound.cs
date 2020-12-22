using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOutputNotFound : IEventHandler
    {
        void OnOutputNotFound(OutputNotFoundEvent OutputNotFoundEvent);
    }
}