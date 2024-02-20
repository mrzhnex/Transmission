using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientOutputMuteStatusChanged : IEventHandler
    {
        void OnClientOutputMuteStatusChanged(ClientOutputMuteStatusChangedEvent clientOutputMuteStatusChangedEvent);
    }
}