using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputEvent : Event
    {
        public byte[] Data { get; set; } = new byte[0];
        public OutputEvent(byte[] Data)
        {
            this.Data = Manage.Application.ScaleVolume(Data, Manage.ApplicationManager.Current.ClientSettings.OutputVolumeValue);
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            if (Manage.ApplicationManager.Current.ClientSettings.OutputMuteStatus && !Manage.Application.IsPlayingAudio)
                return;
            ((IEventHandlerOutput)handler).OnOutput(this);
        }
    }
}