using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerInputVolumeChanged : IEventHandler
    {
        void OnInputVolumeChanged(InputVolumeChangedEvent inputVolumeChangedEvent);
    }
}