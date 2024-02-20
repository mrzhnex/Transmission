using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClientOutputVolumeChanged : IEventHandler
    {
        void OnClientOutputVolumeChanged(ClientOutputVolumeChangedEvent clientOutputVolumeChangedEvent);
    }
}