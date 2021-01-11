﻿using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Core.Application;
using Core.Events;
using Core.Handlers;
using Core.Main;

namespace Client
{
    public partial class MainWindow : Window, IEventHandlerConnect, IEventHandlerDisconnect,
                IEventHandlerInputFound, IEventHandlerOutputFound, IEventHandlerInputNotFound, IEventHandlerOutputNotFound, IEventHandlerSettingsLoaded, IEventHandlerClientUpdate
    {
        #region Main
        public static MainWindow MainWindowInstance { get; set; }
        public bool IsConnectWindowOpened { get; set; } = false;
        public bool IsHelpWindowOpened { get; set; } = false;
        public bool IsSettingsWindowOpened { get; set; } = false;
        public Core.Server.Client Client { get; set; } = new Core.Server.Client(new IPEndPoint(IPAddress.Any, 0), 0, nameof(Client), new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second), nameof(Client), nameof(Client), false);
        public Spectrum InputSpectrum { get; set; } = new Spectrum(); 
        public Spectrum OutputSpectrum { get; set; } = new Spectrum();
        public MainWindow()
        {
            InitializeComponent();
            MainWindowInstance = this;
            Manage.Application = new Application();
            Manage.Application.AddEventHandlers(this); 
            Manage.ApplicationManager.Load();
            OutputSpectrum = (Spectrum)OutputSpectrumControl.DataContext;
            InputSpectrum = (Spectrum)InputSpectrumControl.DataContext;
        }

        public void OnSettingsLoaded(SettingsLoadedEvent settingsLoadedEvent)
        {
            ClientName.Text = settingsLoadedEvent.Settings.ClientName;
            InputVolume.Value = settingsLoadedEvent.Settings.InputVolumeValue;
            OutputVolume.Value = settingsLoadedEvent.Settings.OutputVolumeValue;
            if (settingsLoadedEvent.Settings.OutputMuteStatus)
            {
                OutputMuteStatus.Content = FindResource("SpeakerCrossed");
            }
            if (settingsLoadedEvent.Settings.InputMuteStatus)
            {
                InputMuteStatus.Content = FindResource("MicrophoneCrossed");
            }
        }
        #endregion

        #region Buttons
        private void Record_Click(object sender, RoutedEventArgs e)
        {
            if (Client.Record.IsRecording)
                Client.Record.Save();
            else
                Client.Record.Start();
        }

        private void RecordPause_Click(object sender, RoutedEventArgs e)
        {
            if (Client.Record.IsRecording)
                Client.Record.Pause();
            else
                Client.Record.Play();
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (Manage.ClientSession == null)
            {
                if (!IsConnectWindowOpened)
                {
                    IsConnectWindowOpened = true;
                    ConnectWindow connectWindow = new ConnectWindow
                    {
                        Owner = this
                    };
                    connectWindow.Show();
                }
            }
            else
            {
                Manage.ClientSession.Disconnect("The decision of the client");
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSettingsWindowOpened)
            {
                IsSettingsWindowOpened = true;
                SettingsWindow settingsWindow = new SettingsWindow
                {
                    Owner = this
                };
                settingsWindow.Show();
            }
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (!IsHelpWindowOpened)
            {
                IsHelpWindowOpened = true;
                HelpWindow helpWindow = new HelpWindow
                {
                    Owner = this
                };
                helpWindow.Show();
            }
        }
        private void PlayPrevious_Click(object sender, RoutedEventArgs e)
        {

        }
        private void PlayNext_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Clients_Click(object sender, RoutedEventArgs e)
        {

        }
        private void OutputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OutputMuteStatus.Content == FindResource("Speaker"))
            {
                OutputMuteStatus.Content = FindResource("SpeakerCrossed");
                Manage.EventManager.ExecuteEvent<IEventHandlerOutputMuteStatusChanged>(new OutputMuteStatusChangedEvent(true));
            }
            else
            {
                OutputMuteStatus.Content = FindResource("Speaker");
                Manage.EventManager.ExecuteEvent<IEventHandlerOutputMuteStatusChanged>(new OutputMuteStatusChangedEvent(false));
            }
        }
        private void InputMuteStatus_Click(object sender, RoutedEventArgs e)
        {
            if (InputMuteStatus.Content == FindResource("Microphone"))
            {
                InputMuteStatus.Content = FindResource("MicrophoneCrossed");
                Manage.EventManager.ExecuteEvent<IEventHandlerInputMuteStatusChanged>(new InputMuteStatusChangedEvent(true));
            }
            else
            {
                InputMuteStatus.Content = FindResource("Microphone");
                Manage.EventManager.ExecuteEvent<IEventHandlerInputMuteStatusChanged>(new InputMuteStatusChangedEvent(false));
            }
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
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Events
        private void ClientName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text != null && (sender as TextBox).Text.Length > 0)
            {
                Manage.ApplicationManager.Current.ClientName = (sender as TextBox).Text;
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Client.Record.Save();
            Manage.EventManager.ExecuteEvent<IEventHandlerShutdown>(new ShutdownEvent());
            base.OnClosing(e);
        }
        private void InputMuteStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (((sender as CheckBox).IsChecked == null || (bool)(sender as CheckBox).IsChecked) == Manage.ApplicationManager.Current.InputMuteStatus)
                return;
            Manage.EventManager.ExecuteEvent<IEventHandlerInputMuteStatusChanged>(new InputMuteStatusChangedEvent((sender as CheckBox).IsChecked == null || (bool)(sender as CheckBox).IsChecked));
        }
        private void OutputMuteStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (((sender as CheckBox).IsChecked == null || (bool)(sender as CheckBox).IsChecked) == Manage.ApplicationManager.Current.OutputMuteStatus)
                return;
            Manage.EventManager.ExecuteEvent<IEventHandlerOutputMuteStatusChanged>(new OutputMuteStatusChangedEvent((sender as CheckBox).IsChecked == null || (bool)(sender as CheckBox).IsChecked));
        }
        private void InputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerInputVolumeChanged>(new InputVolumeChangedEvent((float)e.NewValue));
        }
        private void OutputVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerOutputVolumeChanged>(new OutputVolumeChangedEvent((float)e.NewValue));
        }
        private void ProgramName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion

        #region Client-Server
        public void OnClientUpdate(ClientUpdateEvent clientUpdateEvent)
        {
            Status.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Status.Text = clientUpdateEvent.ConnectionInfo.ClientStatus.ToString(); }));
            SessionName.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SessionName.Text = clientUpdateEvent.ConnectionInfo.SessionName; }));
            ServerName.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ServerName.Text = clientUpdateEvent.ConnectionInfo.ServerName; }));
            SessionTime.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SessionTime.Text = $"{clientUpdateEvent.ConnectionInfo.SessionStartTimeSpan}"; }));
            CurrentTime.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { CurrentTime.Text = $"{new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)}"; }));
        }
        public void OnConnect(ConnectEvent connectEvent)
        {
            Connect.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Connect.Content = FindResource("ConnectionTwo"); }));
        }
        public void OnDisconnect(DisconnectEvent disconnectEvent)
        {
            Connect.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { Connect.Content = FindResource("Connection"); }));
            SessionTime.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { SessionTime.Text = "00:00:00"; }));
            CurrentTime.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { CurrentTime.Text = "00:00:00"; }));
        }
        #endregion

        #region Device
        public void OnInputFound(InputFoundEvent inputFoundEvent)
        {
            InputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { InputMuteStatus.IsEnabled = true; }));
        }
        public void OnOutputFound(OutputFoundEvent outputFoundEvent)
        {
            OutputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { OutputMuteStatus.IsEnabled = true; }));
        }
        public void OnInputNotFound(InputNotFoundEvent InputNotFoundEvent)
        {
            InputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { InputMuteStatus.Content = FindResource("MicrophoneCrossed"); }));
            InputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { InputMuteStatus.IsEnabled = false; }));
        }
        public void OnOutputNotFound(OutputNotFoundEvent OutputNotFoundEvent)
        {
            OutputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { OutputMuteStatus.Content = FindResource("SpeakerCrossed"); }));
            OutputMuteStatus.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { OutputMuteStatus.IsEnabled = false; }));
        }
        #endregion
    }
}