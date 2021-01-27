using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerPreviousStep : IEventHandler
    {
        void OnPreviousStep(PreviousStepEvent previousStepEvent);
    }
}