using Core.Handlers;

namespace Core.Events
{
    public class OpenEvent : Event
    {
        public string Password { get; set; } = string.Empty;
        public OpenEvent(string Password)
        {
            this.Password = Password;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerOpen)handler).OnOpen(this);
        }
    }
}