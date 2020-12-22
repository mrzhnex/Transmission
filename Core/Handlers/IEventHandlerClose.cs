using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerClose : IEventHandler
    {
        void OnClose(CloseEvent closeEvent);
    }
}