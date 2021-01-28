using Core.Events;
using Core.Handlers;
using Core.Main;
using NAudio.Wave;
using System;
using System.Windows.Threading;

namespace Server
{
    public class Application : Core.Main.Application, IEventHandlerServerUpdate, IEventHandlerInput, IEventHandlerNextStep
    {
        private BufferedWaveProvider BufferedWaveProviderAudio { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);
        public void OnServerUpdate(ServerUpdateEvent serverUpdateEvent)
        {
            MainWindow.MainWindowInstance.UpdateServerInfo(serverUpdateEvent.Clients);
            MainWindow.MainWindowInstance.CurrentTime.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { MainWindow.MainWindowInstance.CurrentTime.Text = $"{new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)}"; }));
        }

        protected override bool CanInput()
        {
            return true;
        }
        protected override bool CanOutput()
        {
            return true;
        }
        protected override void PrepareInput() { }
        protected override void PrepareOutput() { }

        protected override TimeSpan BufferedDuration()
        {
            return BufferedWaveProviderAudio.BufferedDuration;
        }

        protected override void ClearBuffer()
        {
            BufferedWaveProviderAudio.ClearBuffer();
        }

        protected override void AudioPlaybackStopped()
        {
            MainWindow.MainWindowInstance.Play.Dispatcher.Invoke(new Action(() => MainWindow.MainWindowInstance.Play.Content = MainWindow.MainWindowInstance.FindResource("Play")));
        }

        public void OnInput(InputEvent inputEvent)
        {
            if (IsPlayingAudio)
            {
                BufferedWaveProviderAudio.AddSamples(inputEvent.Data, 0, inputEvent.Data.Length);
                if (Manage.ServerSession != null)
                    Manage.ServerSession.Server.AddAudio(inputEvent.Data);
            }
        }

        public void OnNextStep(NextStepEvent nextStepEvent)
        {
            if (BufferedWaveProviderAudio.BufferedDuration.TotalSeconds > 0.1)
                BufferedWaveProviderAudio.ClearBuffer();
        }
    }
}