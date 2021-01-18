using Core.Events;
using Core.Handlers;
using Core.Main;
using NAudio.Wave;
using System.Linq;

namespace Client
{
    public class Application : Core.Main.Application, IEventHandlerLog, IEventHandlerOutput
    {
        private WaveIn WaveIn { get; set; } = new WaveIn()
        {
            WaveFormat = Manage.DefaultInformation.WaveFormat
        };
        private WaveOut WaveOut { get; set; } = new WaveOut();
        private BufferedWaveProvider BufferStream { get; set; } = new BufferedWaveProvider(Manage.DefaultInformation.WaveFormat);

        #region Override
        protected override void PrepareOutput()
        {
            if (!CanOutput())
            {
                return;
            }
            WaveOut.DesiredLatency = 80;
            WaveOut.Init(BufferStream);
            WaveOut.Play();
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
            if (Manage.ApplicationManager.Current.ClientSettings.InputMuteStatus)
                MainWindow.MainWindowInstance.InputSpectrum.ProcessData(e.Buffer.ToList(), true);
            else
                MainWindow.MainWindowInstance.InputSpectrum.ProcessData(e.Buffer.ToList());
        }

        public void OnLog(LogEvent logEvent)
        {
            if (logEvent.LogLevel < LogLevel.Info)
                return;
        }

        public void OnOutput(OutputEvent outputEvent)
        {
            BufferStream.AddSamples(outputEvent.Data, 0, outputEvent.Data.Length);
            MainWindow.MainWindowInstance.Client.AddAudio(outputEvent.Data);
            if (Manage.ApplicationManager.Current.ClientSettings.OutputMuteStatus)
                MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(outputEvent.Data.ToList(), true);
            else
                MainWindow.MainWindowInstance.OutputSpectrum.ProcessData(outputEvent.Data.ToList());
        }
    }
}