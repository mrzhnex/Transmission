using Core.Handlers;

namespace Core.Events
{
    public class DisconnectEvent : Event
    {
        public string Reason { get; set; } = string.Empty;
        public DisconnectEvent(string Reason)
        {
            this.Reason = Reason;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerDisconnect)handler).OnDisconnect(this);
        }
    }
}