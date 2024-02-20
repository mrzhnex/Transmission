using Core.Application;
using Core.Handlers;

namespace Core.Events
{
    public class SettingsLoadedEvent : Event
    {
        public Settings Settings { get; set; }
        public SettingsLoadedEvent(Settings Settings)
        {
            this.Settings = Settings;
        }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerSettingsLoaded)handler).OnSettingsLoaded(this);
        }
    }
}