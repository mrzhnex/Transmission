using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerLog : IEventHandler
    {
        void OnLog(LogEvent logEvent);
    }
}