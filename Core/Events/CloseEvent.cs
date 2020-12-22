using Core.Handlers;

namespace Core.Events
{
    public class CloseEvent : Event
    {
        public string Reason { get; set; } = string.Empty;
        public CloseEvent(string Reason)
        {
            this.Reason = Reason;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClose)handler).OnClose(this);
        }
    }
}