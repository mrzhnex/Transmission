using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOutputVolumeChanged : IEventHandler
    {
        void OnOutputVolumeChanged(OutputVolumeChangedEvent outputVolumeChangedEvent);
    }
}