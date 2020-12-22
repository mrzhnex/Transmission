using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputFoundEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add("Input found", LogType.Application, LogLevel.Info);
            ((IEventHandlerInputFound)handler).OnInputFound(this);
        }
    }
}