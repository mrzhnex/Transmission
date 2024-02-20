using Core.Handlers;

namespace Core.Events
{
    public class ClientOutputVolumeChangedEvent : Event
    {
        public int Id { get; set; } = 0;
        public float OutputVolumeValue { get; set; } = 0.0f;
        public ClientOutputVolumeChangedEvent(int Id, float OutputVolumeValue)
        {
            this.Id = Id;
            this.OutputVolumeValue = OutputVolumeValue;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientOutputVolumeChanged)handler).OnClientOutputVolumeChanged(this);
        }
    }
}