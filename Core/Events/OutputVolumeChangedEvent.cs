using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputVolumeChangedEvent : Event
    {
        public float OutputVolumeValue { get; set; } = 0.0f;
        public OutputVolumeChangedEvent(float OutputVolumeValue)
        {
            this.OutputVolumeValue = OutputVolumeValue;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            Manage.Logger.Add($"Set {nameof(OutputVolumeValue)} to {OutputVolumeValue}", LogType.Application, LogLevel.Info);
            Manage.ApplicationManager.ClientSettings.OutputVolumeValue = OutputVolumeValue;
            ((IEventHandlerOutputVolumeChanged)handler).OnOutputVolumeChanged(this);
        }
    }
}