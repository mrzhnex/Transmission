using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Server
{
    public partial class ClientWindow : Window, IEventHandlerClientUpdate
    {
        public ClientWindow(ConnectionInfo ConnectionInfo)
        {
            DataContext = ConnectionInfo;
            InitializeComponent();
            Manage.Application.AddEventHandlers(this);
        }

        #region Client

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo(-1, "Default", TimeSpan.Zero);
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { connectionInfo = DataContext as ConnectionInfo; }));
            Manage.ServerSession.DisconnectClient(Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == connectionInfo.Id), "Server discision");
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

        public void OnClientUpdate(ClientUpdateEvent clientUpdateEvent)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo(-1, "Default", TimeSpan.Zero);
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { connectionInfo = DataContext as ConnectionInfo; }));
            if (connectionInfo.Id == clientUpdateEvent.ConnectionInfo.Id)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { DataContext = clientUpdateEvent.ConnectionInfo; }));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo(-1, "Default", TimeSpan.Zero);
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { connectionInfo = DataContext as ConnectionInfo; }));
            if (MainWindow.MainWindowInstance.OpenedClients.Contains(connectionInfo.Id))
                MainWindow.MainWindowInstance.OpenedClients.Remove(connectionInfo.Id);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}