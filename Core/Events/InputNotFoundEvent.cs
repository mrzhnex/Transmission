using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputNotFoundEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add("Input not found", LogType.Application, LogLevel.Warn);
            Manage.EventManager.ExecuteEvent<IEventHandlerInputMuteStatusChanged>(new InputMuteStatusChangedEvent(true));
            ((IEventHandlerInputNotFound)handler).OnInputNotFound(this);
        }
    }
}