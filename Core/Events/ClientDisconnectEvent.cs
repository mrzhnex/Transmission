using Core.Handlers;
using Core.Server;

namespace Core.Events
{
    public class ClientDisconnectEvent : Event
    {
        public ConnectionInfo ConnectionInfo { get; set; }
        public ClientDisconnectEvent(ConnectionInfo ConnectionInfo)
        {
            this.ConnectionInfo = ConnectionInfo;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientDisconnect)handler).OnClientDisconnect(this);
        }
    }
}