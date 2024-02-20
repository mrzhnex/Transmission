using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Server
{
    public partial class ClientWindow : Window, IEventHandlerSpectrumUpdate, IEventHandlerFontFamilyChanged
    {
        public SpectrumControl InputSpectrum { get; set; } = new SpectrumControl();

        public int Id { get; set; } = -1;

        public ClientWindow(int Id)
        {
            this.Id = Id;
            InitializeComponent();
            InputSpectrum = InputSpectrumControl;
            Manage.Application.AddEventHandlers(this);
            foreach (TextBlock textBlock in new TextBlock[] { Username, ClientStatus, ConnectionTimeSpan, Ip })
            {
                SetBinding(textBlock);
            }
            ConnectionInfo connectionInfo = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo;
            if (connectionInfo != null)
            {
                OutputMuteStatus.Content = connectionInfo.ServerOutputMuteStatus ? FindResource("SpeakerCrossed") : FindResource("Speaker");
                InputMuteStatus.Content = connectionInfo.ServerInputMuteStatus ? FindResource("MicrophoneCrossed") : FindResource("Microphone");
                InputVolume.Value = connectionInfo.ServerInputVolumeValue;
                OutputVolume.Value = connectionInfo.ServerOutputVolumeValue;
            }
        }

        private void SetBinding(TextBlock textBlock)
        {
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, new Binding
            {
                Source = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(textBlock.Name)
            });
        }

        #region ClientButtons
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Manage.ServerSession.DisconnectClient(Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id), "Server discision");
            Close();
        }
        private void OutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OutputMuteStatus.Content == FindResource("Speaker"))
            {
                Manage.EventManager.ExecuteEvent<IEventHandlerClientOutputMuteStatusChanged>(new ClientOutputMuteStatusChangedEvent(Id, true));
                OutputMuteStatus.Content = FindResource("SpeakerCrossed");
            }
            else
            {
                Manage.EventManager.ExecuteEvent<IEventHandlerClientOutputMuteStatusChanged>(new ClientOutputMuteStatusChangedEvent(Id, false));
                OutputMuteStatus.Content = FindResource("Speaker");
            }
        }
        private void InputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (InputMuteStatus.Content == FindResource("Microphone"))
            {
                Manage.EventManager.ExecuteEvent<IEventHandlerClientInputMuteStatusChanged>(new ClientInputMuteStatusChangedEvent(Id, true));
                InputMuteStatus.Content = FindResource("MicrophoneCrossed");
            }
            else
            {
                Manage.EventManager.ExecuteEvent<IEventHandlerClientInputMuteStatusChanged>(new ClientInputMuteStatusChangedEvent(Id, false));
                InputMuteStatus.Content = FindResource("Microphone");
            }
        }
        #endregion

        #region WindowButtons
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Deploy_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Deploy.Content = FindResource("Deploy");
                WindowState = WindowState.Normal;
            }
            else
            {
                Deploy.Content = FindResource("DeployTwo");
                WindowState = WindowState.Maximized;
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Events
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.MainWindowInstance.ClientWindows.FirstOrDefault(x => x.Id == Id) != default)
                MainWindow.MainWindowInstance.ClientWindows.Remove(MainWindow.MainWindowInstance.ClientWindows.FirstOrDefault(x => x.Id == Id));
        }
        private void InputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerClientInputVolumeChanged>(new ClientInputVolumeChangedEvent(Id, (float)e.NewValue));
        }
        private void OutputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerClientOutputVolumeChanged>(new ClientOutputVolumeChangedEvent(Id, (float)e.NewValue));
        }
        public void OnSpectrumUpdate(SpectrumUpdateEvent spectrumUpdateEvent)
        {
            if (Id == spectrumUpdateEvent.Id)
            {
                InputSpectrum.ProcessData(spectrumUpdateEvent.Data, spectrumUpdateEvent.Silent);
            }
        }
        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }
        #endregion
    }
}