using Core.Events;
using Core.Handlers;
using Core.Main;
using System.Net;
using System.Windows;

namespace Client
{
    public partial class ConnectWindow : Window, IEventHandlerFontFamilyChanged
    {
        public ConnectWindow()
        {
            InitializeComponent();
            Manage.Application.AddEventHandlers(this);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (Manage.ClientSession != null)
            {
                Manage.ClientSession.Disconnect("The decision of the client");
            }
            string[] ipAndPort = IpAddressField.Text.Split(':');
            if (ipAndPort.Length != 2)
            {
                Manage.Logger.Add("Некорректный IP адрес сервера!", LogType.Application, LogLevel.Warn);
                return;
            }
            if (!IPAddress.TryParse(ipAndPort[0], out IPAddress iPAddress))
            {
                Manage.Logger.Add("Некорректный IP адрес сервера!", LogType.Application, LogLevel.Warn);
                return;
            }
            if (!int.TryParse(ipAndPort[1], out int port))
            {
                Manage.Logger.Add("Некорректный порт сервера!", LogType.Application, LogLevel.Warn);
                return;
            }
            Manage.Logger.Add($"Попытка подключиться к {IpAddressField.Text}", LogType.Application, LogLevel.Info);
            Manage.ClientSession = new Core.Client.Session(iPAddress, Manage.ApplicationManager.Current.ClientSettings.ClientName, port, PasswordField.Text);
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.MainWindowInstance.IsConnectWindowOpened = false;
        }

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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new System.Windows.Media.FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }
    }
}