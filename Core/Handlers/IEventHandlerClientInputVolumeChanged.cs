using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientInputVolumeChanged : IEventHandler
    {
        void OnClientInputVolumeChanged(ClientInputVolumeChangedEvent clientInputVolumeChangedEvent);
    }
}