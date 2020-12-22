using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientsOutputMuteStatusChanged : IEventHandler
    {
        void OnClientsOutputMuteStatusChanged(ClientsOutputMuteStatusChangedEvent clientsOutputMuteStatusChangedEvent);
    }
}