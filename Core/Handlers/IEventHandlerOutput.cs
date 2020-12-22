using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOutput : IEventHandler
    {
        void OnOutput(OutputEvent outputEvent);
    }
}