using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputEvent : Event
    {
        public byte[] Data { get; set; } = new byte[0];
        public InputEvent(byte[] Data)
        {
            this.Data = Manage.Application.ScaleVolume(Data, Manage.ApplicationManager.Current.InputVolumeValue);
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            if (Manage.ApplicationManager.Current.InputMuteStatus)
                return;
            ((IEventHandlerInput)handler).OnInput(this);
        }
    }
}