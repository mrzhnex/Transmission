using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;

namespace Server
{
    public partial class MainWindow : Window, IEventHandlerOpen, IEventHandlerClose, IEventHandlerClientDisconnect
    {
        #region Main
        public static MainWindow MainWindowInstance { get; private set; }
        private ObservableCollection<ConnectionInfo> ServersConnectionInfos { get; set; } = new ObservableCollection<ConnectionInfo>();
        private ObservableCollection<ConnectionInfo> ModeratorsConnectionInfos { get; set; } = new ObservableCollection<ConnectionInfo>();
        private ObservableCollection<ConnectionInfo> SpeakersConnectionInfos { get; set; } = new ObservableCollection<ConnectionInfo>();
        private ObservableCollection<ConnectionInfo> ListenersConnectionInfos { get; set; } = new ObservableCollection<ConnectionInfo>();

        public ConnectionInfo DragConnectionInfo { get; set; } = null;

        public List<int> OpenedClients { get; set; } = new List<int>();

        private void ProgramName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        public MainWindow()
        {
            InitializeComponent();
            MainWindowInstance = this;
            Manage.Application = new Application();
            Manage.Application.AddEventHandlers(this);
            Manage.ApplicationManager.Load();
            Speakers.ItemsSource = SpeakersConnectionInfos;
            Listeners.ItemsSource = ListenersConnectionInfos;
            Moderators.ItemsSource = ModeratorsConnectionInfos;
        }
        #endregion

        #region Buttons
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Manage.ServerSession != null)
            {
                Manage.ServerSession.Close("Administrator's decision");
            }
            else
            {
                Manage.ServerSession = new Session(Manage.DefaultInformation.ServerPort, Manage.DefaultInformation.SessionName, true);
            }
        }
        private void Record_Click(object sender, RoutedEventArgs e)
        {
            if (Manage.ServerSession != null)
            {
                if (Manage.ServerSession.Server.Record.IsRecording)
                    Manage.ServerSession.Server.Record.Save();
                else
                    Manage.ServerSession.Server.Record.Start();
            }
        }
        private void RecordPause_Click(object sender, RoutedEventArgs e)
        {
            if (Manage.ServerSession != null)
            {
                if (Manage.ServerSession.Server.Record.IsRecording)
                    Manage.ServerSession.Server.Record.Pause();
                else
                    Manage.ServerSession.Server.Record.Play();
            }
        }
        private void PlayPrevious_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PlayNext_Click(object sender, RoutedEventArgs e)
        {

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
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Other Events
        protected override void OnClosing(CancelEventArgs e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerShutdown>(new ShutdownEvent());
            base.OnClosing(e);
            System.Windows.Application.Current.Shutdown();
        }
        private void InputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void OutputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        #endregion

        #region Client-Server
        public void OnOpen(OpenEvent openEvent)
        {
            Open.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Open.Content = FindResource("ConnectionTwo"); }));
        }
        public void OnClose(CloseEvent closeEvent)
        {
            UpdateServerInfo(new List<Client>());
            Open.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Open.Content = FindResource("Connection"); }));
            Time.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Time.Text = "00:00:00 - 00:00:00"; }));

        }
        public void OnClientDisconnect(ClientDisconnectEvent clientDisconnectEvent)
        {
            if (ListenersConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault() != default)
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ListenersConnectionInfos.Remove(ListenersConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault()); }));
            if (SpeakersConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault() != default)
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SpeakersConnectionInfos.Remove(SpeakersConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault()); }));
            if (ModeratorsConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault() != default)
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ModeratorsConnectionInfos.Remove(ModeratorsConnectionInfos.Where(x => x.Id == clientDisconnectEvent.ConnectionInfo.Id).FirstOrDefault()); }));
        }
        public void UpdateServerInfo(List<Client> clients)
        {
            foreach (Client client in clients)
            {
                switch (client.ConnectionInfo.ClientStatus)
                {
                    case ClientStatus.Listener:
                        if (ListenersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() == default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ListenersConnectionInfos.Add(client.ConnectionInfo); }));
                        if (SpeakersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SpeakersConnectionInfos.Remove(SpeakersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        if (ModeratorsConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ModeratorsConnectionInfos.Remove(ModeratorsConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        break;
                    case ClientStatus.Speaker:
                        if (SpeakersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() == default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SpeakersConnectionInfos.Add(client.ConnectionInfo); }));
                        if (ListenersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ListenersConnectionInfos.Remove(ListenersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        if (ModeratorsConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ModeratorsConnectionInfos.Remove(ModeratorsConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        break;
                    case ClientStatus.Moderator:
                        if (ModeratorsConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() == default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ModeratorsConnectionInfos.Add(client.ConnectionInfo); }));
                        if (ListenersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ListenersConnectionInfos.Remove(ListenersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        if (SpeakersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault() != default)
                            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SpeakersConnectionInfos.Remove(SpeakersConnectionInfos.Where(x => x.Id == client.ConnectionInfo.Id).FirstOrDefault()); }));
                        break;
                }
            }
            RefreshLists();
        }
        #endregion

        #region Helper
        private void RemoveDragConnectionInfoFromList()
        {
            switch (DragConnectionInfo.ClientStatus)
            {
                case ClientStatus.Listener:
                    ListenersConnectionInfos.Remove(DragConnectionInfo);
                    break;
                case ClientStatus.Speaker:
                    SpeakersConnectionInfos.Remove(DragConnectionInfo);
                    break;
                case ClientStatus.Moderator:
                    ModeratorsConnectionInfos.Remove(DragConnectionInfo);
                    break;
            }
        }
        private void RefreshLists()
        {
            Servers.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Servers.Items.Refresh(); }));
            Moderators.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Moderators.Items.Refresh(); }));
            Speakers.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Speakers.Items.Refresh(); }));
            Listeners.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Listeners.Items.Refresh(); }));
        }
        #endregion

        #region Drag and Drop
        private void Moderators_Drop(object sender, DragEventArgs e)
        {
            RemoveDragConnectionInfoFromList();
            DragConnectionInfo.SetClientStatus(ClientStatus.Moderator);
            ModeratorsConnectionInfos.Add(DragConnectionInfo);
            Manage.ServerSession.DragDropSetMuteStatus(DragConnectionInfo.Id);
            DragConnectionInfo = null;
            RefreshLists();
        }
        private void Speakers_Drop(object sender, DragEventArgs e)
        {
            RemoveDragConnectionInfoFromList();
            DragConnectionInfo.SetClientStatus(ClientStatus.Speaker);
            SpeakersConnectionInfos.Add(DragConnectionInfo);
            Manage.ServerSession.DragDropSetMuteStatus(DragConnectionInfo.Id);
            DragConnectionInfo = null;
            RefreshLists();
        }
        private void Listeners_Drop(object sender, DragEventArgs e)
        {
            RemoveDragConnectionInfoFromList();
            DragConnectionInfo.SetClientStatus(ClientStatus.Listener);
            ListenersConnectionInfos.Add(DragConnectionInfo);
            Manage.ServerSession.DragDropSetMuteStatus(DragConnectionInfo.Id);
            DragConnectionInfo = null;
            RefreshLists();
        }
        #endregion

        #region Input and Output
        private void ModeratorsOutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (ModeratorsOutputMuteStatus.Content == FindResource("Speaker3"))
            {
                ModeratorsOutputMuteStatus.Content = FindResource("SpeakerCrossed3");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(true, ClientStatus.Moderator));
            }
            else
            {
                ModeratorsOutputMuteStatus.Content = FindResource("Speaker3");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(false, ClientStatus.Moderator));
            }
        }
        private void ModeratorsInputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (ModeratorsInputMuteStatus.Content == FindResource("Microphone3"))
            {
                ModeratorsInputMuteStatus.Content = FindResource("MicrophoneCrossed3");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(true, ClientStatus.Moderator));
            }
            else
            {
                ModeratorsInputMuteStatus.Content = FindResource("Microphone3");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(false, ClientStatus.Moderator));
            }
        }
        private void SpeakersOutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (SpeakersOutputMuteStatus.Content == FindResource("Speaker2"))
            {
                SpeakersOutputMuteStatus.Content = FindResource("SpeakerCrossed2");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(true, ClientStatus.Speaker));
            }
            else
            {
                SpeakersOutputMuteStatus.Content = FindResource("Speaker2");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(false, ClientStatus.Speaker));
            }
        }
        private void SpeakersInputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (SpeakersInputMuteStatus.Content == FindResource("Microphone2"))
            {
                SpeakersInputMuteStatus.Content = FindResource("MicrophoneCrossed2");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(true, ClientStatus.Speaker));
            }
            else
            {
                SpeakersInputMuteStatus.Content = FindResource("Microphone2");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(false, ClientStatus.Speaker));
            }
        }
        private void ListenersOutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (ListenersOutputMuteStatus.Content == FindResource("Speaker"))
            {
                ListenersOutputMuteStatus.Content = FindResource("SpeakerCrossed");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(true, ClientStatus.Listener));
            }
            else
            {
                ListenersOutputMuteStatus.Content = FindResource("Speaker");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsOutputMuteStatusChanged>(new ClientsOutputMuteStatusChangedEvent(false, ClientStatus.Listener));
            }
        }
        private void ListenersInputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (ListenersInputMuteStatus.Content == FindResource("Microphone"))
            {
                ListenersInputMuteStatus.Content = FindResource("MicrophoneCrossed");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(true, ClientStatus.Listener));
            }
            else
            {
                ListenersInputMuteStatus.Content = FindResource("Microphone");
                Manage.EventManager.ExecuteEvent<IEventHandlerClientsInputMuteStatusChanged>(new ClientsInputMuteStatusChangedEvent(false, ClientStatus.Listener));
            }
        }
        #endregion

        private void Help_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}