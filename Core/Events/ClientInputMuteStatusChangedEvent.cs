using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class ClientInputMuteStatusChangedEvent : Event
    {
        public int Id { get; set; } = 0;
        public bool InputMuteStatus { get; set; } = false;
        public ClientInputMuteStatusChangedEvent(int Id, bool InputMuteStatus)
        {
            this.Id = Id;
            this.InputMuteStatus = InputMuteStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set client with id {Id} {nameof(InputMuteStatus)} to {InputMuteStatus}", LogType.Application, LogLevel.Info);
            ((IEventHandlerClientInputMuteStatusChanged)handler).OnClientInputMuteStatusChanged(this);
        }
    }
}