using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;
using System;
using System.Linq;
using System.Net;
using Xamarin.Forms;

namespace Mobile
{
    public partial class MainPage : ContentPage
    {
        private static Label StaticSystemMessageField { get; set; }

        public MainPage()
        {
            InitializeComponent();
            StaticSystemMessageField = SystemMessageField;
            Manage.Application = new Application();
            Manage.Application.AddEventHandlers((Application)Manage.Application);
            (Manage.Application as Application).GetValidSampleRates();
        }

        public static void ShowSystemMessage(string message)
        {
            StaticSystemMessageField.Dispatcher.BeginInvokeOnMainThread(new Action(() => { StaticSystemMessageField.Text = message + "\n" + StaticSystemMessageField.Text; }));
        }

        private void Connect_Clicked(object sender, EventArgs e)
        {
            if (Manage.ClientSession != null)
            {
                ShowSystemMessage("Вы уже подключены к серверу!");
                return;
            }
            string[] ipAndPort = IpAddressField.Text.Split(':');
            if (ipAndPort.Length != 2)
            {
                ShowSystemMessage("Некорректный IP адрес сервера!");
                return;
            }
            if (!IPAddress.TryParse(ipAndPort[0], out IPAddress iPAddress))
            {
                ShowSystemMessage("Некорректный IP адрес сервера!");
                return;
            }
            if (!int.TryParse(ipAndPort[1], out int port))
            {
                ShowSystemMessage("Некорректный порт сервера!");
                return;
            }
            byte[] key = new byte[Manage.DefaultInformation.DataLength];
            char[] array = KeyField.Text.ToArray();
            for (int i = 0; i < key.Length; i++)
            {
                if (!byte.TryParse(array[i].ToString(), out key[i]))
                {
                    ShowSystemMessage("Некорректный ключ!");
                    return;
                }
            }
            ShowSystemMessage("Попытка подключиться к " + IpAddressField.Text);
            Manage.ClientSession = new Core.Client.Session(iPAddress, port, key);
        }

        private void InputMuteStatus_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!(sender as CheckBox).IsChecked && !(Manage.Application as Application).IsRecording && !(Manage.Application as Application).TryToStartRecording())
            {
                (sender as CheckBox).IsChecked = false;
                return;
            }
            Manage.EventManager.ExecuteEvent<IEventHandlerInputMuteStatusChanged>(new InputMuteStatusChangedEvent((sender as CheckBox).IsChecked));
        }
    }
}