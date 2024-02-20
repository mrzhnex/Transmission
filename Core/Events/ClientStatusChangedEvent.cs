using Core.Handlers;
using Core.Server;

namespace Core.Events
{
    public class ClientStatusChangedEvent : Event
    {
        public ClientStatus ClientStatus { get; set; } = ClientStatus.Listener;
        public ClientStatusChangedEvent(ClientStatus ClientStatus)
        {
            this.ClientStatus = ClientStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientStatusChanged)handler).OnClientStatusChanged(this);
        }
    }
}