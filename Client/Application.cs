using Core.Events;
using Core.Handlers;
using Core.Main;
using NAudio.Wave;
using System.Threading;

namespace Client
{
    public class Application : Core.Main.Application, IEventHandlerLog, IEventHandlerOutput, IEventHandlerNextStep, IEventHandlerPreviousStep
    {
        private WaveIn WaveIn { get; set; } = new WaveIn()
        {
            WaveFormat = Manage.DefaultInformation.WaveFormat
        };
        private WaveOut WaveOut { get; set; } = new WaveOut();
        private WaveOut WaveOutAudio { get; set; } = new WaveOut();
        private BufferedWaveProvider BufferedWaveProvider { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);
        private BufferedWaveProvider BufferedWaveProviderAudio { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);

        public Application()
        {
            new Thread(AudioPlay).Start();
        }

        #region Override
        protected override void PrepareOutput()
        {
            if (!CanOutput())
            {
                return;
            }
            WaveOut.DesiredLatency = 80;
            WaveOut.Init(BufferedWaveProvider);
            WaveOut.Play();

            WaveOutAudio.DesiredLatency = 80;
            WaveOutAudio.Init(BufferedWaveProviderAudio);
            WaveOutAudio.Play();

            IsOutputPrepared = true;
        }
        protected override void PrepareInput()
        {
            if (!CanInput())
            {
                return;
            }
            WaveIn.DataAvailable += Input;
            WaveIn.StartRecording();
            IsInputPrepared = true;
        }
        protected override bool CanOutput()
        {
            return WaveOut.DeviceCount != 0;
        }
        protected override bool CanInput()
        {
            return WaveIn.DeviceCount != 0;
        }
        #endregion

        private void Input(object sender, WaveInEventArgs e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerInput>(new InputEvent(e.Buffer));
            MainWindow.MainWindowInstance.Client.AddAudio(e.Buffer);
            MainWindow.MainWindowInstance.InputSpectrum.ProcessData(e.Buffer, Manage.ApplicationManager.Current.ClientSettings.InputMuteStatus);
        }

        public void OnLog(LogEvent logEvent)
        {
            if (logEvent.LogLevel < LogLevel.Info)
                return;
        }

        public void OnOutput(OutputEvent outputEvent)
        {
            BufferedWaveProvider.AddSamples(outputEvent.Data, 0, outputEvent.Data.Length);
            MainWindow.MainWindowInstance.Client.AddAudio(outputEvent.Data);
            MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(outputEvent.Data, Manage.ApplicationManager.Current.ClientSettings.OutputMuteStatus);
        }

        private void AudioPlay()
        {
            byte[] data;
            while (Manage.Logger.ActiveLog)
            {
                if (CanOutput() && IsPlayingAudio)
                {
                    if (AudioData.Count <= Length + Index && BufferedWaveProviderAudio.BufferedDuration.TotalSeconds == 0.0)
                    {
                        SetIndexToZero();
                        SetLengthToDefault();
                        IsPlayingAudio = false;
                        MainWindow.MainWindowInstance.Play.Dispatcher.Invoke(new System.Action(() => MainWindow.MainWindowInstance.Play.Content = MainWindow.MainWindowInstance.FindResource("Play")));
                        Manage.Logger.Add($"{nameof(IsPlayingAudio)} now is {IsPlayingAudio}", LogType.Client, LogLevel.Debug);
                        continue;
                    }
                    if (AudioData.Count <= Length + Index || BufferedWaveProviderAudio.BufferedDuration.TotalSeconds > 0.1)
                        continue;

                    NextStep();

                    data = AudioData.GetRange(Index, Length).ToArray();
                    BufferedWaveProviderAudio.AddSamples(data, 0, data.Length);
                    MainWindow.MainWindowInstance.Client.AddAudio(data);
                    MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(data, Manage.ApplicationManager.Current.ClientSettings.OutputMuteStatus);
                }
                else
                {
                    if (BufferedWaveProviderAudio.BufferedDuration.TotalSeconds > 0.1)
                        BufferedWaveProviderAudio.ClearBuffer();
                }
            }
        }

        public void OnNextStep(NextStepEvent nextStepEvent)
        {
            if (BufferedWaveProviderAudio.BufferedDuration.TotalSeconds > 0.1)
                BufferedWaveProviderAudio.ClearBuffer();
            MainWindow.MainWindowInstance.OutputSpectrum.ClearPreValues();
        }

        public void OnPreviousStep(PreviousStepEvent previousStepEvent)
        {
            MainWindow.MainWindowInstance.OutputSpectrum.ClearPreValues();
        }
    }
}