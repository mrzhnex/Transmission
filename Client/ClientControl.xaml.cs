using Core.Client;
using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client
{
    public partial class ClientControl : UserControl, IEventHandlerFontFamilyChanged, IEventHandlerClientStatusChanged, IEventHandlerOtherClientStatusChanged
    {
        private int Id { get; set; } = 0;
        private List<ComboBoxItem> ComboBoxModeratorItems { get; set; } = new List<ComboBoxItem>()
        {
            new ComboBoxItem() { Content = Core.Server.ClientStatus.Moderator }
        };
        private List<ComboBoxItem> ComboBoxItems { get; set; } = new List<ComboBoxItem>()
        {
            new ComboBoxItem() { Content = Core.Server.ClientStatus.Listener },
            new ComboBoxItem() { Content = Core.Server.ClientStatus.Speaker }
        };

        public ClientStatus OldClientStatus { get; set; } = Core.Server.ClientStatus.Listener;

        public ClientControl()
        {
            InitializeComponent();
            Manage.Application.AddEventHandlers(this);
        }

        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBoxItem)((ComboBox)sender).SelectedItem == null)
                return;
            ClientStatus clientStatus = (ClientStatus)Enum.Parse(typeof(ClientStatus), ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString());
            if (clientStatus == Core.Server.ClientStatus.Moderator)
                return;
            Manage.EventManager.ExecuteEvent<IEventHandlerUpdateClientStatus>(new UpdateClientStatusEvent(new ClientInfo(Id, string.Empty, clientStatus, TimeSpan.Zero)));
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Id = ((ClientInfo)DataContext).Id;
            OldClientStatus = ((ClientInfo)DataContext).ClientStatus;
            if (MainWindow.MainWindowInstance.ClientStatus == Core.Server.ClientStatus.Moderator)
            {
                if (OldClientStatus == Core.Server.ClientStatus.Moderator)
                {
                    ClientStatus.ItemsSource = ComboBoxModeratorItems;
                    ClientStatus.SelectedItem = ComboBoxModeratorItems.FirstOrDefault();
                    ClientStatus.IsEnabled = false;
                }
                else
                {
                    ClientStatus.ItemsSource = ComboBoxItems;
                    ClientStatus.SelectedItem = ComboBoxItems.FirstOrDefault(x => (ClientStatus)Enum.Parse(typeof(ClientStatus), x.Content.ToString()) == OldClientStatus);
                }
            }
            else
            {
                if (OldClientStatus == Core.Server.ClientStatus.Moderator)
                {
                    ClientStatus.ItemsSource = ComboBoxModeratorItems;
                    ClientStatus.SelectedItem = ComboBoxModeratorItems.FirstOrDefault();
                }
                else
                {
                    ClientStatus.ItemsSource = ComboBoxItems;
                    ClientStatus.SelectedItem = ComboBoxItems.FirstOrDefault(x => (ClientStatus)Enum.Parse(typeof(ClientStatus), x.Content.ToString()) == OldClientStatus);
                }
                ClientStatus.IsEnabled = false;
            }
        }

        public void OnClientStatusChanged(ClientStatusChangedEvent clientStatusChangedEvent)
        {
            switch (clientStatusChangedEvent.ClientStatus)
            {
                case Core.Server.ClientStatus.Moderator:
                    if (OldClientStatus != Core.Server.ClientStatus.Moderator)
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.IsEnabled = true));
                    break;
                default:
                    ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.IsEnabled = false));
                    break;
            }
        }

        public void OnOtherClientStatusChanged(OtherClientStatusChanged otherClientStatusChanged)
        {
            if (otherClientStatusChanged.Id == Id)
            {
                OldClientStatus = otherClientStatusChanged.ClientStatus;
                switch (otherClientStatusChanged.ClientStatus)
                {
                    case Core.Server.ClientStatus.Moderator:
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.ItemsSource = ComboBoxModeratorItems));
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.SelectedItem = ComboBoxModeratorItems.FirstOrDefault()));
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.IsEnabled = false));
                        break;
                    default:
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.ItemsSource = ComboBoxItems));
                        ClientStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => ClientStatus.SelectedItem = ComboBoxItems.FirstOrDefault(x => (ClientStatus)Enum.Parse(typeof(ClientStatus), x.Content.ToString()) == OldClientStatus)));
                        break;
                }
            }
        }
    }
}