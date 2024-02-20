using Core.Handlers;

namespace Core.Events
{
    public class ClientInputVolumeChangedEvent : Event
    {
        public int Id { get; set; } = 0;
        public float InputVolumeValue { get; set; } = 0.0f;
        public ClientInputVolumeChangedEvent(int Id, float InputVolumeValue)
        {
            this.Id = Id;
            this.InputVolumeValue = InputVolumeValue;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerClientInputVolumeChanged)handler).OnClientInputVolumeChanged(this);
        }
    }
}