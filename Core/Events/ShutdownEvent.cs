using Core.Handlers;

namespace Core.Events
{
    public class ShutdownEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerShutdown)handler).OnShutdown(this);
        }
    }
}