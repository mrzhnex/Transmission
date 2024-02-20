using Core.Client;
using Core.Handlers;
using System;

namespace Core.Events
{
    public class UpdateClientStatusEvent : Event
    {
        public ClientInfo ClientInfo { get; set; } = new ClientInfo(0, string.Empty, Server.ClientStatus.Listener, TimeSpan.Zero);
        public UpdateClientStatusEvent(ClientInfo ClientInfo)
        {
            this.ClientInfo = ClientInfo;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerUpdateClientStatus)handler).OnUpdateClientStatus(this);
        }
    }
}