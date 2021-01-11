using Core.Events;
using Core.Handlers;
using System.Collections.Generic;
using System.Threading;

namespace Core.Main
{
    public abstract class Application : IEventHandlerInputMuteStatusChanged, IEventHandlerOutputMuteStatusChanged, IEventHandlerShutdown, IEventHandlerInputVolumeChanged, IEventHandlerOutputVolumeChanged
    {
        protected internal bool IsInputPrepared { get; set; } = false;
        protected internal bool IsOutputPrepared { get; set; } = false;
        protected internal bool IsInputNotFound { get; set; } = true;
        protected internal bool IsOutputNotFound { get; set; } = true;
        protected internal abstract bool CanInput();
        protected internal abstract bool CanOutput();
        protected internal abstract void PrepareInput();
        protected internal abstract void PrepareOutput();
        private Thread Thread { get; set; }

        public Application()
        {
            Thread = new Thread(OutputThread);
            Thread.Start();
            Manage.EventManager.AddEventHandlers(this);
        }

        public void AddEventHandlers(IEventHandler eventHandler)
        {
            Manage.EventManager.AddEventHandlers(eventHandler);
        }

        internal byte[] ScaleVolume(byte[] data, float volume)
        {
            for (int i = 0; i < data.Length / 2; ++i)
            {
                short sample = (short)((data[i * 2 + 1] << 8) | data[i * 2]);
                sample = (short)(sample * volume + 0.5);
                data[i * 2 + 1] = (byte)(sample >> 8);
                data[i * 2] = (byte)(sample & 0xff);
            }
            return data;
        }

        private void OutputThread()
        {
            while (Manage.Logger.ActiveLog)
            {
                if (IsInputNotFound && CanInput())
                {
                    IsInputNotFound = false;
                    Manage.EventManager.ExecuteEvent<IEventHandlerInputFound>(new InputFoundEvent());
                    if (!IsInputPrepared)
                        PrepareInput();
                }
                if (!IsInputNotFound && !CanInput())
                {
                    IsInputNotFound = true;
                    Manage.EventManager.ExecuteEvent<IEventHandlerInputNotFound>(new InputNotFoundEvent());
                }
                if (IsOutputNotFound && CanOutput())
                {
                    IsOutputNotFound = false;
                    Manage.EventManager.ExecuteEvent<IEventHandlerOutputFound>(new OutputFoundEvent());
                    if (!IsOutputPrepared)
                        PrepareOutput();
                }
                if (!IsOutputNotFound && !CanOutput())
                {
                    IsOutputNotFound = true;
                    Manage.EventManager.ExecuteEvent<IEventHandlerOutputNotFound>(new OutputNotFoundEvent());
                }
            }
        }

        public void OnShutdown(ShutdownEvent shutdownEvent)
        {
            Manage.ApplicationManager.Save();
            if (Manage.ServerSession != null)
            {
                Manage.ServerSession.Close("The app is closing");
            }
            if (Manage.ClientSession != null)
            {
                Manage.ClientSession.Disconnect("The app is closing");
            }
            Manage.Logger.Add($"The app was closed", LogType.Application, LogLevel.Info);
            Manage.Logger.ActiveLog = false;
            Manage.Logger.SaveAllLogs();
        }
        public void OnInputMuteStatusChanged(InputMuteStatusChangedEvent inputMuteStatusChangedEvent) { }
        public void OnOutputMuteStatusChanged(OutputMuteStatusChangedEvent outputMuteStatusChangedEvent) { }
        public void OnInputVolumeChanged(InputVolumeChangedEvent inputVolumeChangedEvent) { }
        public void OnOutputVolumeChanged(OutputVolumeChangedEvent outputVolumeChangedEvent) { }
    }
}