using Core.Events;
using Core.Handlers;
using Core.Main;
using NAudio.Wave;
using System;

namespace Client
{
    public class Application : Core.Main.Application, IEventHandlerOutput, IEventHandlerNextStep, IEventHandlerPreviousStep, IEventHandlerInput, IEventHandlerOutputMuteStatusChanged
    {
        private WaveIn WaveIn { get; set; } = new WaveIn()
        {
            WaveFormat = Manage.DefaultInformation.WaveFormat
        };
        private WaveOut WaveOut { get; set; } = new WaveOut();
        private WaveOut WaveOutAudio { get; set; } = new WaveOut();
        private BufferedWaveProvider BufferedWaveProvider { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);
        private BufferedWaveProvider BufferedWaveProviderAudio { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);


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
            WaveOutAudio.Volume = Manage.ApplicationManager.ClientSettings.OutputMuteStatus ? 0.0f : 1.0f;

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
            if (!IsPlayingAudio)
            {
                Manage.EventManager.ExecuteEvent<IEventHandlerInput>(new InputEvent(e.Buffer));
            }
        }


        public override void OnOutputMuteStatusChanged(OutputMuteStatusChangedEvent outputMuteStatusChangedEvent)
        {
            if (outputMuteStatusChangedEvent.OutputMuteStatus)
            {
                WaveOutAudio.Volume = 0.0f;
                MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(new byte[Manage.DefaultInformation.DataLength], Manage.ApplicationManager.ClientSettings.OutputMuteStatus);
            }
            else
                WaveOutAudio.Volume = 1.0f;
        }


        public void OnOutput(OutputEvent outputEvent)
        {
            if (IsPlayingAudio)
            {
                BufferedWaveProviderAudio.AddSamples(outputEvent.Data, 0, outputEvent.Data.Length);
                MainWindow.MainWindowInstance.Client.AddAudio(outputEvent.Data);
            }
            else
            {
                BufferedWaveProvider.AddSamples(outputEvent.Data, 0, outputEvent.Data.Length);
            }
            MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(outputEvent.Data, Manage.ApplicationManager.ClientSettings.OutputMuteStatus);
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

        protected override TimeSpan BufferedDuration()
        {
            return BufferedWaveProviderAudio.BufferedDuration;
        }

        protected override void ClearBuffer()
        {
            BufferedWaveProviderAudio.ClearBuffer();
        }

        public void OnInput(InputEvent inputEvent)
        {
            MainWindow.MainWindowInstance.Client.AddAudio(inputEvent.Data);
            MainWindow.MainWindowInstance.InputSpectrum.ProcessData(inputEvent.Data, Manage.ApplicationManager.ClientSettings.InputMuteStatus);
        }

        protected override void AudioPlaybackStopped()
        {
            MainWindow.MainWindowInstance.Play.Dispatcher.Invoke(new Action(() => MainWindow.MainWindowInstance.Play.Content = MainWindow.MainWindowInstance.FindResource("Play")));
        }
    }
}