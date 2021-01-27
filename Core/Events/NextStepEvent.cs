using Core.Handlers;

namespace Core.Events
{
    public class NextStepEvent : Event
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerNextStep)handler).OnNextStep(this);
        }
    }
}