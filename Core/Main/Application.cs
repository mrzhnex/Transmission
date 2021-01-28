using Core.Events;
using Core.Handlers;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Main
{
    public abstract class Application : IEventHandlerInputMuteStatusChanged, IEventHandlerOutputMuteStatusChanged, IEventHandlerShutdown, IEventHandlerInputVolumeChanged, IEventHandlerOutputVolumeChanged, IEventHandlerLog, IEventHandlerClientUpdate
    {
        protected internal bool IsInputPrepared { get; set; } = false;
        protected internal bool IsOutputPrepared { get; set; } = false;
        protected internal bool IsInputNotFound { get; set; } = true;
        protected internal bool IsOutputNotFound { get; set; } = true;
        protected internal abstract bool CanInput();
        protected internal abstract bool CanOutput();
        protected internal abstract void PrepareInput();
        protected internal abstract void PrepareOutput();
        protected internal List<byte> AudioData { get; set; } = new List<byte>();
        public bool IsPlayingAudio { get; protected set; } = false;
        public bool IsAudioLoaded { get; set; } = false;
        protected internal int Length { get; set; } = Manage.DefaultInformation.DataLength;
        protected internal int Index { get; set; } = 0;

        #region Main
        public Application()
        {
            new Thread(OutputThread).Start();
            new Thread(AudioPlay).Start();
            Manage.EventManager.AddEventHandlers(this);
        }
        public void AddEventHandlers(IEventHandler eventHandler)
        {
            Manage.EventManager.AddEventHandlers(eventHandler);
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
        #endregion

        #region PlayAudioFile
        public void SwitchIsPlayingAudio()
        {
            IsPlayingAudio = !IsPlayingAudio;
        }
        public void LoadAudioData(string audioFilePath)
        {
            if (audioFilePath == null || audioFilePath == string.Empty || audioFilePath.Length == 0)
                return;
            WaveFileReader waveFileReader = new WaveFileReader(audioFilePath);
            byte[] data = new byte[waveFileReader.Length];
            waveFileReader.Read(data, 0, data.Length);
            AudioData = data.ToList();
            IsAudioLoaded = true;
            Manage.Logger.Add($"Loaded {nameof(AudioData)} from {audioFilePath}", LogType.Application, LogLevel.Info);
        }
        public void NextStep()
        {
            if (AudioData.Count <= Length + Index)
                return;
            bool IsPlayingAudio = this.IsPlayingAudio;
            this.IsPlayingAudio = false;
            Index += Length;
            if (Index + Length > AudioData.Count)
                Length = AudioData.Count - Index;
            this.IsPlayingAudio = IsPlayingAudio;
            Manage.EventManager.ExecuteEvent<IEventHandlerNextStep>(new NextStepEvent());
            Manage.Logger.Add($"Invoke next step", LogType.Application, LogLevel.Info);
        }
        public void PreviousStep()
        {
            if (Index == 0)
                return;
            bool IsPlayingAudio = this.IsPlayingAudio;
            this.IsPlayingAudio = false;
            if (Length != Manage.DefaultInformation.DataLength)
                SetLengthToDefault();
            Index -= Length;
            if (Index < 0)
                SetIndexToZero();
            this.IsPlayingAudio = IsPlayingAudio;
            Manage.EventManager.ExecuteEvent<IEventHandlerPreviousStep>(new PreviousStepEvent());
            Manage.Logger.Add($"Invoke previous step", LogType.Application, LogLevel.Info);
        }
        public void SetLengthToDefault()
        {
            Length = Manage.DefaultInformation.DataLength;
            Manage.Logger.Add($"Set {nameof(Length)} to {Length}", LogType.Application, LogLevel.Debug);
        }
        public void SetIndexToZero()
        {
            Index = 0;
            Manage.Logger.Add($"Set {nameof(Index)} to {Index}", LogType.Application, LogLevel.Debug);
        }
        protected internal abstract TimeSpan BufferedDuration();
        protected internal abstract void ClearBuffer();
        protected internal abstract void AudioPlaybackStopped();
        private void AudioPlay()
        {
            byte[] data;
            while (Manage.Logger.ActiveLog)
            {
                if (IsPlayingAudio)
                {
                    if (AudioData.Count <= Length + Index && BufferedDuration().TotalSeconds == 0.0)
                    {
                        SetIndexToZero();
                        SetLengthToDefault();
                        IsPlayingAudio = false;
                        AudioPlaybackStopped();
                        Manage.Logger.Add($"{nameof(IsPlayingAudio)} now is {IsPlayingAudio}", LogType.Client, LogLevel.Debug);
                        continue;
                    }
                    if (AudioData.Count <= Length + Index || BufferedDuration().TotalSeconds > 0.1)
                        continue;

                    NextStep();

                    data = AudioData.GetRange(Index, Length).ToArray();

                    Manage.EventManager.ExecuteEvent<IEventHandlerInput>(new InputEvent(data));
                    Manage.EventManager.ExecuteEvent<IEventHandlerOutput>(new OutputEvent(data));

                }
                else
                {
                    if (BufferedDuration().TotalSeconds > 0.1)
                        ClearBuffer();
                }
            }
        }
        #endregion

        #region Events
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
        public void OnClientUpdate(ClientUpdateEvent clientUpdateEvent)
        {
            Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientUpdateEvent.ConnectionInfo.Id).ConnectionInfo.UpdateConnectionTimeSpan();
        }
        public void OnInputMuteStatusChanged(InputMuteStatusChangedEvent inputMuteStatusChangedEvent) { }
        public void OnOutputMuteStatusChanged(OutputMuteStatusChangedEvent outputMuteStatusChangedEvent) { }
        public void OnInputVolumeChanged(InputVolumeChangedEvent inputVolumeChangedEvent) { }
        public void OnOutputVolumeChanged(OutputVolumeChangedEvent outputVolumeChangedEvent) { }
        public void OnLog(LogEvent logEvent) { }
        #endregion
    }
}