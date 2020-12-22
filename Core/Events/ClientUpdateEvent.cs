using Core.Handlers;
using Core.Server;

namespace Core.Events
{
    public class ClientUpdateEvent : Event
    {
        public ConnectionInfo ConnectionInfo { get; set; }
        public ClientUpdateEvent(ConnectionInfo ConnectionInfo)
        {
            this.ConnectionInfo = ConnectionInfo;
        }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientUpdate)handler).OnClientUpdate(this);
        }
    }
}