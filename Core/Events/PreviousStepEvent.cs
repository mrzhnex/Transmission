using Core.Handlers;

namespace Core.Events
{
    public class PreviousStepEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPreviousStep)handler).OnPreviousStep(this);
        }
    }
}