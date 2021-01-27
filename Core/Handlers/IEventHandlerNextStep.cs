using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerNextStep : IEventHandler
    {
        void OnNextStep(NextStepEvent nextStepEvent);
    }
}