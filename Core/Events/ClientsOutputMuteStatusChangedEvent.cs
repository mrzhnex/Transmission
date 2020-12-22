using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class ClientsOutputMuteStatusChangedEvent : Event
    {
        public bool OutputMuteStatus { get; set; } = false;
        public Server.ClientStatus ClientStatus { get; set; } = Server.ClientStatus.Listener;
        public ClientsOutputMuteStatusChangedEvent(bool OutputMuteStatus, Server.ClientStatus ClientStatus)
        {
            this.OutputMuteStatus = OutputMuteStatus;
            this.ClientStatus = ClientStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set all {nameof(ClientStatus)} to {OutputMuteStatus}", LogType.Application, LogLevel.Info);
            ((IEventHandlerClientsOutputMuteStatusChanged)handler).OnClientsOutputMuteStatusChanged(this);
        }
    }
}