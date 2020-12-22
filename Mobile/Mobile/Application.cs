using Android.Media;
using Core.Events;
using Core.Handlers;
using Core.Main;
using System.Threading;

namespace Mobile
{
    public class Application : Core.Main.Application, IEventHandlerOutput, IEventHandlerLog, IEventHandlerInputMuteStatusChanged
    {
        public bool IsRecording { get; private set; } = false;
        private int SampleRate { get; set; } = 8000;
        private byte[] AudioBuffer { get; set; } = new byte[Manage.DefaultInformation.DataLength];
        private AudioRecord AudioRecord { get; set; }
        private AudioTrack AudioTrack { get; set; }

        public Application()
        {
            AudioTrack = new AudioTrack(Stream.Music, SampleRate, ChannelOut.Mono, Encoding.Pcm16bit, AudioBuffer.Length, AudioTrackMode.Stream);
            AudioRecord = new AudioRecord(AudioSource.Mic, SampleRate, ChannelIn.Mono, Encoding.Pcm16bit, AudioBuffer.Length);
            AudioTrack.Play();
        }

        public bool TryToStartRecording()
        {
            try
            {
                AudioRecord.StartRecording();
                IsRecording = true;
                Thread thread = new Thread(ReadAudio);
                thread.Start();
                Manage.Logger.Add("Start recording...", LogType.Application, LogLevel.Debug);
                return true;
            }
            catch (Java.Lang.IllegalStateException)
            {
                return false;
            }
        }

        public void GetValidSampleRates()
        {
            foreach (int rate in new int[] { 8000, 11025, 16000, 22050, 44100 })
            {
                int bufferSize = AudioRecord.GetMinBufferSize(rate, ChannelIn.Mono, Encoding.Pcm16bit);
                if (bufferSize > 0)
                {
                    Manage.Logger.Add($"Good rate is {rate} buffer is {bufferSize}", LogType.Application, LogLevel.Debug);
                }
            }
        }
        public void OnLog(LogEvent logEvent)
        {
            MainPage.ShowSystemMessage(logEvent.Message);
        }

        private void ReadAudio()
        {
            while (true)
            {
                AudioBuffer = new byte[Manage.DefaultInformation.DataLength];
                AudioRecord.Read(AudioBuffer, 0, AudioBuffer.Length);
                Manage.EventManager.ExecuteEvent<IEventHandlerInput>(new InputEvent(AudioBuffer));
            }
        }
        public void OnOutput(OutputEvent outputEvent)
        {
            AudioTrack.Write(outputEvent.Data, 0, outputEvent.Data.Length);
            Manage.Logger.Add($"Play: {Manage.GetUlongFromBuffer(outputEvent.Data)}", LogType.Application, LogLevel.Debug);
        }

        public void OnInputMuteStatusChanged(InputMuteStatusChangedEvent changeInputStatusEvent)
        {
            //InputMuteStatus = changeInputStatusEvent.InputMuteStatus;
            Manage.Logger.Add($"Set Input status to {changeInputStatusEvent.InputMuteStatus}", LogType.Application, LogLevel.Info);
        }

        protected override bool CanInput()
        {
            throw new System.NotImplementedException();
        }

        protected override bool CanOutput()
        {
            throw new System.NotImplementedException();
        }

        protected override void PrepareInput()
        {
            throw new System.NotImplementedException();
        }

        protected override void PrepareOutput()
        {
            throw new System.NotImplementedException();
        }
    }
}