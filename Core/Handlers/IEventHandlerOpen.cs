using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerOpen : IEventHandler
    {
        void OnOpen(OpenEvent openEvent);
    }
}