using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class ClientOutputMuteStatusChangedEvent : Event
    {
        public int Id { get; set; } = 0;
        public bool OutputMuteStatus { get; set; } = false;
        public ClientOutputMuteStatusChangedEvent(int Id, bool OutputMuteStatus)
        {
            this.Id = Id;
            this.OutputMuteStatus = OutputMuteStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set client with id {Id} {nameof(OutputMuteStatus)} to {OutputMuteStatus}", LogType.Application, LogLevel.Info);
            ((IEventHandlerClientOutputMuteStatusChanged)handler).OnClientOutputMuteStatusChanged(this);
        }
    }
}