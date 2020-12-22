using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOutputFound : IEventHandler
    {
        void OnOutputFound(OutputFoundEvent outputFoundEvent);
    }
}