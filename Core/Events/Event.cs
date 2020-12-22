using Core.Handlers;

namespace Core.Events
{
    public abstract class Event
    {
        public abstract void ExecuteHandler(IEventHandler handler);
    }
}