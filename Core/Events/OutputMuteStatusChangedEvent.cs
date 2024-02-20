using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputMuteStatusChangedEvent : Event
    {
        public bool OutputMuteStatus { get; set; } = false;
        public OutputMuteStatusChangedEvent(bool OutputMuteStatus)
        {
            this.OutputMuteStatus = OutputMuteStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set {nameof(OutputMuteStatus)} to {OutputMuteStatus}", LogType.Application, LogLevel.Info);
            Manage.ApplicationManager.ClientSettings.OutputMuteStatus = OutputMuteStatus;
            ((IEventHandlerOutputMuteStatusChanged)handler).OnOutputMuteStatusChanged(this);
        }
    }
}