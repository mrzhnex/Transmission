using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputMuteStatusChangedEvent : Event
    {
        public bool InputMuteStatus { get; set; } = false;
        public InputMuteStatusChangedEvent(bool InputMuteStatus)
        {
            this.InputMuteStatus = InputMuteStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set {nameof(InputMuteStatus)} to {InputMuteStatus}", LogType.Application, LogLevel.Info);
            Manage.ApplicationManager.ClientSettings.InputMuteStatus = InputMuteStatus;
            ((IEventHandlerInputMuteStatusChanged)handler).OnInputMuteStatusChanged(this);
        }
    }
}