using Core.Handlers;

namespace Core.Events
{
    public class ConnectEvent : Event
    {
        public string Ip { get; set; } = string.Empty;
        public ConnectEvent(string Ip)
        {
            this.Ip = Ip;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerConnect)handler).OnConnect(this);
        }
    }
}