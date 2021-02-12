using Core.Handlers;

namespace Core.Events
{
    public class ShouldLogChangedEvent : Event
    {
        public bool ShouldLog { get; set; } = false;
        public ShouldLogChangedEvent(bool ShouldLog)
        {
            this.ShouldLog = ShouldLog;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerShouldLogChanged)handler).OnShouldLogChanged(this);
        }
    }
}