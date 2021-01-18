using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputVolumeChangedEvent : Event
    {
        public float InputVolumeValue { get; set; } = 0.0f;
        public InputVolumeChangedEvent(float InputVolumeValue)
        {
            this.InputVolumeValue = InputVolumeValue;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set {nameof(InputVolumeValue)} to {InputVolumeValue}", LogType.Application, LogLevel.Info);
            Manage.ApplicationManager.Current.ClientSettings.InputVolumeValue = InputVolumeValue;
            ((IEventHandlerInputVolumeChanged)handler).OnInputVolumeChanged(this);
        }
    }
}