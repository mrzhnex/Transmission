using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerUpdateClientStatus : IEventHandler
    {
        void OnUpdateClientStatus(UpdateClientStatusEvent updateClientStatusEvent);
    }
}