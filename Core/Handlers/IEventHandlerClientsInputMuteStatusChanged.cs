using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientsInputMuteStatusChanged : IEventHandler
    {
        void OnClientsInputMuteStatusChanged(ClientsInputMuteStatusChangedEvent clientsInputMuteStatusChangedEvent);
    }
}