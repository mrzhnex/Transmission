using Core.Handlers;

namespace Core.Events
{
    public class SpectrumUpdateEvent : Event
    {
        public int Id { get; set; } = 0;
        public byte[] Data { get; set; } = new byte[0];
        public bool Silent { get; set; } = false;

        public SpectrumUpdateEvent(int Id, byte[] Data, bool Silent = false)
        {
            this.Id = Id;
            this.Data = Data;
            this.Silent = Silent;
        }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerSpectrumUpdate)handler).OnSpectrumUpdate(this);
        }
    }
}