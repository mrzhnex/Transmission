﻿using System;
using Core.Events;
using Core.Handlers;
using Core.Main;
using Mono.Nat;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Core.Client;

namespace Core.Server
{
    public class Session : IEventHandlerClientsInputMuteStatusChanged, IEventHandlerClientsOutputMuteStatusChanged, IEventHandlerInput, IEventHandlerClientInputMuteStatusChanged, 
        IEventHandlerClientOutputMuteStatusChanged, IEventHandlerClientInputVolumeChanged, IEventHandlerClientOutputVolumeChanged
    {
        private bool ShouldRedirectPort { get; set; } = true;
        private ServerStage ServerStage { get; set; } = ServerStage.Starting;
        private Socket ListeningSocket { get; set; } = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Thread ListeningThread { get; set; }
        public List<Client> Clients { get; private set; } = new List<Client>();
        public List<Client> DisconnectedClients { get; private set; } = new List<Client>();
        private string Password { get; set; } = string.Empty;
        private Thread CloseThread { get; set; }
        private Thread StartThread { get; set; }
        private Thread SendingThread { get; set; }
        private INatDevice NatDevice { get; set; }
        private int Port { get; set; } = 0;
        public string Name { get; private set; } = nameof(Name);
        public string ServerName { get; private set; } = nameof(ServerName);

        private bool ListenersOutputMuteStatus { get; set; } = false;
        private bool ListenersInputMuteStatus { get; set; } = false;
        private bool SpeakersOutputMuteStatus { get; set; } = false;
        private bool SpeakersInputMuteStatus { get; set; } = false;
        private bool ModeratorsOutputMuteStatus { get; set; } = false;
        private bool ModeratorsInputMuteStatus { get; set; } = false;

        public Client Server { get; set; } = new Client(new IPEndPoint(IPAddress.Any, 0), 0, nameof(Server), new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second), nameof(Name), nameof(ServerName), false, false);

        public Session(int Port, string ServerName, string Password, bool StartServerAnyway = false)
        {
            Manage.Logger.Add("The server is starting...", LogType.Server, LogLevel.Debug);
            this.Port = Port;
            this.Password = Password;
            this.ServerName = ServerName;
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();
            ListeningThread = new Thread(new ThreadStart(Listening));
            StartThread = new Thread(Open);
            CloseThread = new Thread(delegate ()
            {
                Close("The firewall is enabled or upnp is disabled or invalid port");
            });
            SendingThread = new Thread(SendingData);
            Manage.Application.AddEventHandlers(this);
            Server.ConnectionInfo.SetClientStatus(ClientStatus.Moderator);
            Thread.Sleep(1000);
            if (NatDevice == null)
            {
                Manage.Logger.Add($"{nameof(NatDevice)} is null", LogType.Server, LogLevel.Warn);
                ShouldRedirectPort = false;
                Manage.Logger.Add($"{nameof(ShouldRedirectPort)} now is {ShouldRedirectPort}", LogType.Server, LogLevel.Info);
            }
            else
            {
                StartServerAnyway = true;
            }
            if (ShouldRedirectPort)
            {
                CloseServerPort(this.Port, Manage.DefaultInformation.Protocol);
                OpenServerPort(this.Port, Manage.DefaultInformation.Protocol, NatDevice.GetExternalIP().ToString(), Manage.DefaultInformation.PortDescription);
            }
            if (StartServerAnyway)
            {
                ListeningSocket.Bind(new IPEndPoint(IPAddress.Any, this.Port));
                StartThread.Start();
            }
            else
            {
                CloseThread.Start();
            }
        }

        public void SetPassword(string Password)
        {
            this.Password = Password;
        }

        #region Events
        public void OnInput(InputEvent inputEvent)
        {
            AddAudio(inputEvent.Data, Server);
        }
        public void OnClientInputMuteStatusChanged(ClientInputMuteStatusChangedEvent clientInputMuteStatusChangedEvent)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientInputMuteStatusChangedEvent.Id).ConnectionInfo.ServerInputMuteStatus = clientInputMuteStatusChangedEvent.InputMuteStatus;
            if (clientInputMuteStatusChangedEvent.InputMuteStatus)
                Manage.EventManager.ExecuteEvent<IEventHandlerSpectrumUpdate>(new SpectrumUpdateEvent(clientInputMuteStatusChangedEvent.Id, new byte[Manage.DefaultInformation.DataLength]));
        }
        public void OnClientOutputMuteStatusChanged(ClientOutputMuteStatusChangedEvent clientOutputMuteStatusChangedEvent)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientOutputMuteStatusChangedEvent.Id).ConnectionInfo.ServerOutputMuteStatus = clientOutputMuteStatusChangedEvent.OutputMuteStatus;
        }
        public void OnClientsInputMuteStatusChanged(ClientsInputMuteStatusChangedEvent clientsInputMuteStatusChangedEvent)
        {
            switch (clientsInputMuteStatusChangedEvent.ClientStatus)
            {
                case ClientStatus.Listener:
                    ListenersInputMuteStatus = clientsInputMuteStatusChangedEvent.InputMuteStatus;
                    break;
                case ClientStatus.Speaker:
                    SpeakersInputMuteStatus = clientsInputMuteStatusChangedEvent.InputMuteStatus;
                    break;
                case ClientStatus.Moderator:
                    ModeratorsInputMuteStatus = clientsInputMuteStatusChangedEvent.InputMuteStatus;
                    break;
            }
            foreach (Client client in Clients)
            {
                if (client.ConnectionInfo.ClientStatus == clientsInputMuteStatusChangedEvent.ClientStatus)
                {
                    client.ConnectionInfo.ServerInputMuteStatus = clientsInputMuteStatusChangedEvent.InputMuteStatus;
                    if (clientsInputMuteStatusChangedEvent.InputMuteStatus)
                        Manage.EventManager.ExecuteEvent<IEventHandlerSpectrumUpdate>(new SpectrumUpdateEvent(client.ConnectionInfo.Id, new byte[Manage.DefaultInformation.DataLength]));
                }
            }
        }
        public void OnClientsOutputMuteStatusChanged(ClientsOutputMuteStatusChangedEvent clientsOutputMuteStatusChangedEvent)
        {
            switch (clientsOutputMuteStatusChangedEvent.ClientStatus)
            {
                case ClientStatus.Listener:
                    ListenersOutputMuteStatus = clientsOutputMuteStatusChangedEvent.OutputMuteStatus;
                    break;
                case ClientStatus.Speaker:
                    SpeakersOutputMuteStatus = clientsOutputMuteStatusChangedEvent.OutputMuteStatus;
                    break;
                case ClientStatus.Moderator:
                    ModeratorsOutputMuteStatus = clientsOutputMuteStatusChangedEvent.OutputMuteStatus;
                    break;
            }
            foreach (Client client in Clients)
            {
                if (client.ConnectionInfo.ClientStatus == clientsOutputMuteStatusChangedEvent.ClientStatus)
                {
                    client.ConnectionInfo.ServerOutputMuteStatus = clientsOutputMuteStatusChangedEvent.OutputMuteStatus;
                }
            }
        }
        public void OnClientInputVolumeChanged(ClientInputVolumeChangedEvent clientInputVolumeChangedEvent)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientInputVolumeChangedEvent.Id).ConnectionInfo.ServerInputVolumeValue = clientInputVolumeChangedEvent.InputVolumeValue;
        }
        public void OnClientOutputVolumeChanged(ClientOutputVolumeChangedEvent clientOutputVolumeChangedEvent)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientOutputVolumeChangedEvent.Id).ConnectionInfo.ServerInputVolumeValue = clientOutputVolumeChangedEvent.OutputVolumeValue;
        }
        #endregion

        #region Methods
        private void Open()
        {
            while (Manage.ServerSession == null) { }
            Manage.Logger.Add($"{nameof(Password)} is {Password}", LogType.Server, LogLevel.Info);
            Manage.EventManager.ExecuteEvent<IEventHandlerOpen>(new OpenEvent(Password));
            Manage.Logger.Add($"The server is open at {ListeningSocket.LocalEndPoint}", LogType.Server, LogLevel.Info);
            ServerStage = ServerStage.Open;
            ListeningThread.Start();
            SendingThread.Start();
        }
        public void Close(string reason)
        {
            if (ServerStage == ServerStage.Closing)
                return;
            ServerStage = ServerStage.Closing;
            DisconnectAllClients(reason);
            Manage.Logger.Add("Start the server shutdown process...", LogType.Server, LogLevel.Debug);
            Manage.EventManager.ExecuteEvent<IEventHandlerClose>(new CloseEvent(reason));
            Clients.Clear();
            ListeningSocket.Close();
            ListeningSocket.Dispose();
            if (ShouldRedirectPort)
                CloseServerPort(Port, Manage.DefaultInformation.Protocol);
            Server.Record.Save();
            Manage.Logger.Add($"The server closed by {reason}", LogType.Server, LogLevel.Info);
            Manage.ServerSession = null;
        }
        private void SendData(byte[] data, IPEndPoint socket, bool showString = false, bool isKey = false)
        {
            if (data.Length < Manage.DefaultInformation.DataLength)
            {
                byte[] temp = new byte[Manage.DefaultInformation.DataLength];
                for (int i = 0; i < data.Length; i++)
                {
                    temp[i] = data[i];
                }
                data = temp;
            }
            try
            {
                ListeningSocket.SendTo(data, socket);
                Manage.Logger.Add($"Send {(isKey ? "Key" : nameof(data))} {(showString ? Manage.GetStringFromData(data) : Manage.GetUlongFromBuffer(data).ToString())} to the client {socket}", LogType.Server, LogLevel.Trace);
            }
            catch (SocketException ex)
            {
                Manage.Logger.Add($"Catch {nameof(SocketException)} {ex.Message} during send {nameof(data)} {Manage.GetUlongFromBuffer(data)} to the client {socket}", LogType.Server, LogLevel.Error);
            }
        }
        #endregion

        #region Helper
        private bool IsKey(byte[] data)
        {
            if (Manage.GetStringFromData(Manage.ParseKeyFromString(Password)) != Manage.GetStringFromData(data))
                return false;
            return true;
        }
        private Client GetClientBySocket(IPEndPoint socket)
        {
            return Clients.FirstOrDefault(x => x.Socket.Address.Equals(socket.Address) && x.Socket.Port == socket.Port) != default ? Clients.FirstOrDefault(x => x.Socket.Address.Equals(socket.Address) && x.Socket.Port == socket.Port) : null;
        }
        private void DeviceFound(object sender, DeviceEventArgs args)
        {
            NatDevice = args.Device;
            Manage.Logger.Add($"Found {nameof(args.Device)} with {nameof(args.Device.NatProtocol)} {args.Device.NatProtocol}", LogType.Server, LogLevel.Info);
        }
        private void OpenServerPort(int port, Protocol protocol, string ip, string description)
        {
            NatDevice.CreatePortMap(new Mapping(protocol, port, port, 999999999, description));
            Manage.Logger.Add($"Add {protocol} port redirection to {ip}:{port} with {description}", LogType.Server, LogLevel.Debug);
        }
        private void CloseServerPort(int port, Protocol protocol)
        {
            if (NatDevice.GetAllMappings().Where(x => x.Protocol == protocol).FirstOrDefault(x => x.PublicPort == port) != default)
            {
                NatDevice.DeletePortMap(NatDevice.GetAllMappings().Where(x => x.Protocol == protocol).FirstOrDefault(x => x.PublicPort == port));
                Manage.Logger.Add($"Remove {protocol} external port redirection from {port}", LogType.Server, LogLevel.Debug);
            }
            if (NatDevice.GetAllMappings().Where(x => x.Protocol == protocol).FirstOrDefault(x => x.PrivatePort == port) != default)
            {
                NatDevice.DeletePortMap(NatDevice.GetAllMappings().Where(x => x.Protocol == protocol).FirstOrDefault(x => x.PrivatePort == port));
                Manage.Logger.Add($"Remove {protocol} internal port redirection from {port}", LogType.Server, LogLevel.Debug);
            }
        }
        #endregion

        #region Client
        private void DisconnectAllClients(string reason)
        {
            foreach (Client client in Clients.ToList())
            {
                DisconnectClient(client, reason);
            }
        }
        public void DisconnectClient(Client client, string reason)
        {
            Manage.Logger.Add($"Disconnect the client {client.Socket} by {reason}", LogType.Server, LogLevel.Info);
            Clients.Remove(client);
            DisconnectedClients.Add(client);
            Manage.EventManager.ExecuteEvent<IEventHandlerClientDisconnect>(new ClientDisconnectEvent(client.ConnectionInfo));
            SendData(Encoding.UTF8.GetBytes(Manage.DefaultInformation.DisconnectMessage + reason), client.Socket);
        }
        public void DisconnectIpEndPoint(IPEndPoint iPEndPoint, string reason)
        {
            SendData(Encoding.UTF8.GetBytes(Manage.DefaultInformation.DisconnectMessage + reason), iPEndPoint);
            Manage.Logger.Add($"Disconnect the {nameof(iPEndPoint)} {iPEndPoint} by {reason}", LogType.Server, LogLevel.Info);
        }
        private void ConnectClient(IPEndPoint iPEndPoint)
        {
            Client client = new Client(iPEndPoint, Clients.Count + DisconnectedClients.Count, "Username", Server.ConnectionInfo.SessionStartTimeSpan, Name, ServerName, false, false);
            Clients.Add(client);
            SendData(Encoding.UTF8.GetBytes(Password), client.Socket);
            Manage.Logger.Add($"The user {client.Socket} has connected to the server. Create new {nameof(client.ConnectionInfo.Key)} {Manage.GetStringFromData(client.ConnectionInfo.Key())}", LogType.Server, LogLevel.Info);

            new Thread(delegate ()
            {
                CheckVerification(client);
            }).Start();
        }
        private void AddAudio(byte[] data, Client sender)
        {
            if (sender.ConnectionInfo.ClientStatus == ClientStatus.Listener || sender.ConnectionInfo.ServerInputMuteStatus || sender.ConnectionInfo.ServerInputVolumeValue == 0.0f)
                return;
            if (Server.Record.IsRecording)
            {
                Server.AddAudio(data);
            }
            foreach (Client client in Clients)
            {
                if (sender.Socket.Address.Equals(client.Socket.Address) && sender.Socket.Port == client.Socket.Port && !Manage.ApplicationManager.ServerSettings.ShouldMirrorAudio)
                    continue;
                if (client.ConnectionInfo.ServerOutputMuteStatus || client.ConnectionInfo.ServerOutputVolumeValue == 0.0f)
                    continue;
                SendData(Manage.Application.ScaleVolume(Manage.Application.ScaleVolume(data, sender.ConnectionInfo.ServerInputVolumeValue), client.ConnectionInfo.ServerOutputVolumeValue), client.Socket);
            }
            Manage.EventManager.ExecuteEvent<IEventHandlerSpectrumUpdate>(new SpectrumUpdateEvent(sender.ConnectionInfo.Id, data));
        }
        #endregion

        #region Threads
        private void SendingData()
        {
            while (ServerStage == ServerStage.Open)
            {
                foreach (Client client in Clients.ToList())
                {
                    SendData(Manage.ClientInfoBehaviour.GetClientInfosData(Clients.ToList()), client.Socket);
                    Manage.EventManager.ExecuteEvent<IEventHandlerClientUpdate>(new ClientUpdateEvent(client.ConnectionInfo));
                }
                Manage.EventManager.ExecuteEvent<IEventHandlerServerUpdate>(new ServerUpdateEvent(Clients));
                Thread.Sleep(Manage.DefaultInformation.ServerDelay);
            }
        }
        private void CheckVerification(Client client)
        {
            while (Clients.Contains(client))
            {
                switch (client.Stage)
                {
                    case VerificationStage.Connected:
                    case VerificationStage.Validated:
                    case VerificationStage.NonValidated:
                        client.ConnectionInfo.IsVerified = false;
                        SendData(client.ConnectionInfo.Key(), client.Socket, true, true);
                        break;
                    case VerificationStage.Disconnected:
                        DisconnectClient(client, "The client failed verification or timeout");
                        break;
                }
                Thread.Sleep(Manage.DefaultInformation.VerificationUpdateTime);
                if (client.ConnectionInfo.IsVerified || client.Stage == VerificationStage.Disconnected)
                    continue;
                client.DeniedVerification();
            }
        }
        private void Listening()
        {
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
            while (ServerStage == ServerStage.Open)
            {
                if (ListeningSocket.Available <= 0)
                    continue;
                byte[] data = new byte[Manage.DefaultInformation.DataLength];
                try
                {
                    ListeningSocket.ReceiveFrom(data, ref remoteIp);
                }
                catch (SocketException)
                {
                    continue;
                }
                IPEndPoint iPEndPoint = remoteIp as IPEndPoint;

                Client client = GetClientBySocket(iPEndPoint);
                if (client != null)
                {
                    if (client.ConnectionInfo.IsVerificationMessage(data))
                    {
                        client.UpdateVerification(data);
                    }
                    else if (Manage.ClientInfoBehaviour.IsClientInfosMessage(data))
                    {
                        if (client.ConnectionInfo.ClientStatus == ClientStatus.Moderator)
                        {
                            List<ClientInfo> clientInfos = Manage.ClientInfoBehaviour.GetClientInfosFromData(data);
                            foreach (ClientInfo clientInfo in clientInfos)
                            {
                                if (Clients.ToList().FirstOrDefault(x => x.ConnectionInfo.Id == clientInfo.Id) != default)
                                {
                                    Clients.ToList().FirstOrDefault(x => x.ConnectionInfo.Id == clientInfo.Id).ConnectionInfo.SetClientStatus(clientInfo.ClientStatus);
                                }
                            }
                        }
                    }
                    else
                    {   
                        AddAudio(data, client);
                    }
                }
                else
                {
                    if (IsKey(data))
                    {
                        ConnectClient(iPEndPoint);
                    }
                    else
                    {
                        DisconnectIpEndPoint(iPEndPoint, "Wrong password");
                    }
                }
            }
        }
        #endregion
    }
    public enum ServerStage
    {
        Starting, Open, Closing
    }
}