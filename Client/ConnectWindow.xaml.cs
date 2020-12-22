using Core.Main;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Client
{
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            InitializeComponent();
            SetLanguageBinding();
            Closing += ConnectWindow_Closing;
        }

        private void ConnectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.MainWindowInstance.IsConnectWindowOpened = false;
        }

        private void SetLanguageBinding()
        {
            TextBlock[] textBlocks = new TextBlock[]
            {
                Password, Server
            };
            Button[] buttons = new Button[]
            {
                Connect
            };
            foreach (TextBlock textBlock in textBlocks)
            {
                SetLanguageBindingTextBlock(textBlock);
            }
            foreach (Button button in buttons)
            {
                SetLanguageBindingButton(button);
            }
        }
        private void SetLanguageBindingTextBlock(TextBlock textBlock)
        {
            textBlock.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { textBlock.SetBinding(TextBlock.TextProperty, new Binding(textBlock.Name) { Source = Manage.LocalizationManager.Current }); }));
        }
        private void SetLanguageBindingButton(Button button)
        {
            button.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { button.SetBinding(ContentProperty, new Binding(button.Name) { Source = Manage.LocalizationManager.Current }); }));
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
            byte[] key = new byte[Manage.DefaultInformation.DataLength];
            char[] array = PasswordField.Text.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (!byte.TryParse(array[i].ToString(), out key[i]))
                {
                    Manage.Logger.Add("Некорректный ключ!", LogType.Application, LogLevel.Warn);
                    return;
                }
            }
            Manage.Logger.Add($"Попытка подключиться к {IpAddressField.Text}", LogType.Application, LogLevel.Info);
            Manage.ClientSession = new Core.Client.Session(iPAddress, MainWindow.MainWindowInstance.ClientName.Text, port, key);
            Close();
        }
    }
}