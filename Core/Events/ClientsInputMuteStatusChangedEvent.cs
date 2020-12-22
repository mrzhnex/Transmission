using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class ClientsInputMuteStatusChangedEvent : Event
    {
        public bool InputMuteStatus { get; set; } = false;
        public Server.ClientStatus ClientStatus { get; set; } = Server.ClientStatus.Listener;
        public ClientsInputMuteStatusChangedEvent(bool InputMuteStatus, Server.ClientStatus ClientStatus)
        {
            this.InputMuteStatus = InputMuteStatus;
            this.ClientStatus = ClientStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set all {nameof(ClientStatus)} to {InputMuteStatus}", LogType.Application, LogLevel.Info);
            ((IEventHandlerClientsInputMuteStatusChanged)handler).OnClientsInputMuteStatusChanged(this);
        }
    }
}