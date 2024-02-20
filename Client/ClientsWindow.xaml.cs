using Core.Client;
using Core.Events;
using Core.Handlers;
using Core.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Client
{
    public partial class ClientsWindow : Window, IEventHandlerFontFamilyChanged, IEventHandlerClientInfosUpdate, IEventHandlerDisconnect
    {
        private ObservableCollection<ClientInfo> ClientsInfos { get; set; } = new ObservableCollection<ClientInfo>();
        public SortBy SortBy { get; set; } = SortBy.Status;

        public ClientsWindow()
        {
            InitializeComponent();
            Manage.Application.AddEventHandlers(this);
            Clients.ItemsSource = ClientsInfos;
            Sort.ItemsSource = new List<SortBy>() { SortBy.Status, SortBy.Time, SortBy.Alphabet };
        }

        #region Helper
        public void UpdateServerInfo(List<ClientInfo> clients)
        {
            foreach (ClientInfo clientInfo in clients)
            {
                if (ClientsInfos.Where(x => x.Id == clientInfo.Id).FirstOrDefault() == default)
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ClientsInfos.Add(clientInfo); }));
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ClientsInfos.Where(x => x.Id == clientInfo.Id).FirstOrDefault().CompareToNew(clientInfo); }));
                    Manage.EventManager.ExecuteEvent<IEventHandlerOtherClientStatusChanged>(new OtherClientStatusChanged(clientInfo.Id, clientInfo.ClientStatus));
                }
            }
            foreach (ClientInfo clientInfo in ClientsInfos.ToList())
            {
                if (clients.Where(x => x.Id == clientInfo.Id).FirstOrDefault() == default)
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ClientsInfos.Remove(clientInfo); }));
            }
            SortByPriority();
        }
        public void SortByPriority()
        {
            switch (SortBy)
            {
                case SortBy.Alphabet:
                    SortMethod(new AlphabetCompare());
                    break;
                case SortBy.Status:
                    SortMethod(new StatusCompare());
                    break;
                case SortBy.Time:
                    SortMethod(new ConnectionTimeSpanCompare());
                    break;
            }
        }
        public void SortMethod(IComparer<ClientInfo> comparison)
        {
            List<ClientInfo> sortableList = new List<ClientInfo>(ClientsInfos);
            sortableList.Sort(comparison);
            for (int i = 0; i < sortableList.Count - 1; i++)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => ClientsInfos.Move(ClientsInfos.IndexOf(sortableList[i]), i)));
            }
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
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.MainWindowInstance.IsClientsWindowOpened = false;
        }
        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sort.SelectedItem = SortBy;
            SortByPriority();
        }
        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortBy = (SortBy)Enum.Parse(typeof(SortBy), ((ComboBox)sender).SelectedValue.ToString());
            SortByPriority();
        }
        #endregion

        #region Handler Events
        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new System.Windows.Media.FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }
        public void OnClientInfosUpdate(ClientInfosUpdateEvent clientInfosUpdateEvent)
        {
            UpdateServerInfo(clientInfosUpdateEvent.ClientInfos);
        }
        public void OnDisconnect(DisconnectEvent disconnectEvent)
        {
            UpdateServerInfo(new List<ClientInfo>());
        }
        #endregion
    }

    public class AlphabetCompare : IComparer<ClientInfo>
    {
        public int Compare(ClientInfo first, ClientInfo second)
        {
            return string.Compare(first.Username, second.Username);
        }
    }
    public class ConnectionTimeSpanCompare : IComparer<ClientInfo>
    {
        public int Compare(ClientInfo first, ClientInfo second)
        {
            if (first.ConnectionTimeSpan > second.ConnectionTimeSpan)
            {
                return 1;
            }
            else if (first.ConnectionTimeSpan < second.ConnectionTimeSpan)
            {
                return -1;
            }
            return 0;
        }
    }
    public class StatusCompare : IComparer<ClientInfo>
    {
        public int Compare(ClientInfo first, ClientInfo second)
        {
            if (first.ClientStatus > second.ClientStatus)
            {
                return 1;
            }
            else if (first.ClientStatus < second.ClientStatus)
            {
                return -1;
            }
            return 0;
        }
    }

    public enum SortBy
    {
        Time, Status, Alphabet
    }
}