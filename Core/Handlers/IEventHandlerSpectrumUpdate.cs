using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerSpectrumUpdate : IEventHandler
    {
        void OnSpectrumUpdate(SpectrumUpdateEvent spectrumUpdateEvent);
    }
}