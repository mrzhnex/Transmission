using Core.Handlers;
using System.Collections.Generic;

namespace Core.Events
{
    public class ServerUpdateEvent : Event
    {
        public List<Server.Client> Clients { get; set; } = new List<Server.Client>();
        public ServerUpdateEvent(List<Server.Client> Clients)
        {
            this.Clients.AddRange(Clients);
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerServerUpdate)handler).OnServerUpdate(this);
        }
    }
}