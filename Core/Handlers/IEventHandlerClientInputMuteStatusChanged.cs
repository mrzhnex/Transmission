using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientInputMuteStatusChanged : IEventHandler
    {
        void OnClientInputMuteStatusChanged(ClientInputMuteStatusChangedEvent clientInputMuteStatusChangedEvent);
    }
}