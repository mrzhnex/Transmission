using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOutputMuteStatusChanged : IEventHandler
    {
        void OnOutputMuteStatusChanged(OutputMuteStatusChangedEvent outputMuteStatusChangedEvent);
    }
}