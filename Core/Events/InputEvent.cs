using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class InputEvent : Event
    {
        public byte[] Data { get; set; } = new byte[0];
        public bool IsServer { get; set; } = false;
        public InputEvent(byte[] Data, bool IsServer = false)
        {
            this.Data = Manage.Application.ScaleVolume(Data, Manage.ApplicationManager.ClientSettings.InputVolumeValue);
            this.IsServer = IsServer;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            if (Manage.ApplicationManager.ClientSettings.InputMuteStatus && !IsServer)
                return;
            ((IEventHandlerInput)handler).OnInput(this);
        }
    }
}