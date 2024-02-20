using Core.Client;
using Core.Handlers;
using System.Collections.Generic;

namespace Core.Events
{
    public class ClientInfosUpdateEvent : Event
    {
        public List<ClientInfo> ClientInfos { get; set; } = new List<ClientInfo>();
        public ClientInfosUpdateEvent(List<ClientInfo> ClientInfos)
        {
            this.ClientInfos = ClientInfos;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientInfosUpdate)handler).OnClientInfosUpdate(this);
        }
    }
}