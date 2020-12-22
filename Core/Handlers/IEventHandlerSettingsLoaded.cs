using Core.Events;

namespace Core.Handlers
{
    public interface IEventHandlerSettingsLoaded : IEventHandler
    {
        void OnSettingsLoaded(SettingsLoadedEvent settingsLoadedEvent);
    }
}