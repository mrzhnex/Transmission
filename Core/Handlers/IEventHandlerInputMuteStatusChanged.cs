using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerInputMuteStatusChanged : IEventHandler
    {
        void OnInputMuteStatusChanged(InputMuteStatusChangedEvent inputMuteStatusChangedEvent);
    }
}