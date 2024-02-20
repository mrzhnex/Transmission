using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOtherClientStatusChanged : IEventHandler
    {
        void OnOtherClientStatusChanged(OtherClientStatusChanged otherClientStatusChanged);
    }
}