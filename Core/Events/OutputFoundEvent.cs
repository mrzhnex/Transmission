using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputFoundEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add("Output found", LogType.Application, LogLevel.Info);
            ((IEventHandlerOutputFound)handler).OnOutputFound(this);
        }
    }
}