using Core.Application;
using Core.Events;
using Core.Handlers;
using Core.Main;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Server
{
    public partial class ClientWindow : Window, IEventHandlerSpectrumUpdate
    {
        public Spectrum InputSpectrum { get; set; } = new Spectrum();

        public int Id { get; set; } = -1;

        public ClientWindow(int Id)
        {
            this.Id = Id;
            InitializeComponent();
            foreach (TextBlock textBlock in new TextBlock[] { Username, ClientStatus, ConnectionTimeSpan, Ip })
            {
                SetBinding(textBlock);
            }

            InputSpectrum = (Spectrum)InputSpectrumControl.DataContext;
            Manage.Application.AddEventHandlers(this);
            return;
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

        #region Client
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Manage.ServerSession.DisconnectClient(Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id), "Server discision");
            Close();
        }
        private void OutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {

        }
        private void InputMuteStatus_Click(object sender, RoutedEventArgs e)
        {

        }
        private void InputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void OutputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        #endregion

        #region Buttons
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
        public void OnSpectrumUpdate(SpectrumUpdateEvent spectrumUpdateEvent)
        {
            if (Id == spectrumUpdateEvent.Id)
            {
                InputSpectrum.ProcessData(spectrumUpdateEvent.Data, spectrumUpdateEvent.Silent);
            }
        }
        #endregion
    }
}