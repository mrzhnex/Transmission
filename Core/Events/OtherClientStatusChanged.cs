using Core.Handlers;
using Core.Server;

namespace Core.Events
{
    public class OtherClientStatusChanged : Event
    {
        public ClientStatus ClientStatus { get; set; } = ClientStatus.Listener;
        public int Id { get; set; } = 0;
        public OtherClientStatusChanged(int Id, ClientStatus ClientStatus)
        {
            this.Id = Id;
            this.ClientStatus = ClientStatus;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerOtherClientStatusChanged)handler).OnOtherClientStatusChanged(this);
        }
    }
}