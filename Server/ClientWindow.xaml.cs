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
    public partial class ClientWindow : Window, IEventHandlerSpectrumUpdate, IEventHandlerClientUpdate
    {
        public Spectrum InputSpectrum { get; set; } = new Spectrum();

        public int Id { get; set; } = 0;

        public ClientWindow(int Id)
        {
            this.Id = Id;
            InitializeComponent();

            BindingOperations.SetBinding(Username, TextBlock.TextProperty, new Binding
            {
                Source = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(nameof(Username))
            });
            BindingOperations.SetBinding(ClientStatus, TextBlock.TextProperty, new Binding
            {
                Source = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(nameof(ClientStatus))
            });
            BindingOperations.SetBinding(ConnectionTimeSpan, TextBlock.TextProperty, new Binding
            {
                Source = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(nameof(ConnectionTimeSpan))
            });
            InputSpectrum = (Spectrum)InputSpectrumControl.DataContext;
            Manage.Application.AddEventHandlers(this);
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
        #endregion

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.MainWindowInstance.ClientWindows.FirstOrDefault(x => x.Id == Id) != default)
                MainWindow.MainWindowInstance.ClientWindows.Remove(MainWindow.MainWindowInstance.ClientWindows.FirstOrDefault(x => x.Id == Id));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        public void OnSpectrumUpdate(SpectrumUpdateEvent spectrumUpdateEvent)
        {
            if (Id == spectrumUpdateEvent.Id)
            {
                InputSpectrum.ProcessData(spectrumUpdateEvent.Data, spectrumUpdateEvent.Silent);
            }
        }

        public void OnClientUpdate(ClientUpdateEvent clientUpdateEvent)
        {
            if (Id == clientUpdateEvent.ConnectionInfo.Id)
            {
                Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo.UpdateConnectionTimeSpan();
            }
        }
    }
}