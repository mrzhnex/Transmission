using Core.Application;
using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Server
{
    public partial class ClientWindow : Window, IEventHandlerClientUpdate
    {
        public Spectrum InputSpectrum { get; set; } = new Spectrum();
        public Spectrum OutputSpectrum { get; set; } = new Spectrum();

        private int Id { get; set; } = 0;

        public ClientWindow(int Id)
        {
            this.Id = Id;
            InitializeComponent();

            Binding binding = new Binding
            {
                Source = Manage.ServerSession.Clients.FirstOrDefault(x => x.ConnectionInfo.Id == Id).ConnectionInfo,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(nameof(Username))
            };

            BindingOperations.SetBinding(Username, TextBlock.TextProperty, binding);

            Manage.Application.AddEventHandlers(this);
            InputSpectrum = (Spectrum)InputSpectrumControl.DataContext;
            OutputSpectrum = (Spectrum)OutputSpectrumControl.DataContext;
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

        public void OnClientUpdate(ClientUpdateEvent clientUpdateEvent)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.MainWindowInstance.OpenedClients.Contains(Id))
                MainWindow.MainWindowInstance.OpenedClients.Remove(Id);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}