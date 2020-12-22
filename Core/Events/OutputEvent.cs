using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class OutputEvent : Event
    {
        public byte[] Data { get; set; } = new byte[0];
        public OutputEvent(byte[] Data)
        {
            this.Data = Manage.Application.ScaleVolume(Data, Manage.ApplicationManager.Current.OutputVolumeValue);
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            if (Manage.ApplicationManager.Current.OutputMuteStatus)
                return;
            ((IEventHandlerOutput)handler).OnOutput(this);
        }
    }
}