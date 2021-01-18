using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputEvent : Event
    {
        public byte[] Data { get; set; } = new byte[0];
        public InputEvent(byte[] Data)
        {
            this.Data = Manage.Application.ScaleVolume(Data, Manage.ApplicationManager.Current.ClientSettings.InputVolumeValue);
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            if (Manage.ApplicationManager.Current.ClientSettings.InputMuteStatus)
                return;
            ((IEventHandlerInput)handler).OnInput(this);
        }
    }
}