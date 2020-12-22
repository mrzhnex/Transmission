using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputNotFoundEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add("Output not found", LogType.Application, LogLevel.Warn);
            Manage.EventManager.ExecuteEvent<IEventHandlerOutputMuteStatusChanged>(new OutputMuteStatusChangedEvent(true));
            ((IEventHandlerOutputNotFound)handler).OnOutputNotFound(this);
        }
    }
}