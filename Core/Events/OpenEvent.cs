using Core.Handlers;

namespace Core.Events
{
    public class OpenEvent : Event
    {
        public byte[] Key { get; set; } = new byte[0];
        public OpenEvent(byte[] Key)
        {
            this.Key = Key;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerOpen)handler).OnOpen(this);
        }
    }
}