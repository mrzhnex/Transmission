using System;
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

namespace Core.Server
{
    public class Session : IEventHandlerClientsInputMuteStatusChanged, IEventHandlerClientsOutputMuteStatusChanged
    {
        private bool ShouldRedirectPort { get; set; } = true;
        private ServerStage ServerStage { get; set; } = ServerStage.Starting;
        private Socket ListeningSocket { get; set; } = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Thread ListeningThread { get; set; }
        public List<Client> Clients { get; private set; } = new List<Client>();
        public List<Client> DisconnectedClients { get; private set; } = new List<Client>();
        private List<IPAddress> Blacklist { get; set; } = new List<IPAddress>();
        private string Password { get; set; } = string.Empty;
        private Thread CloseThread { get; set; }
        private Thread StartThread { get; set; }
        private Thread SendingThread { get; set; }
        private INatDevice NatDevice { get; set; }
        private int Port { get; set; } = 0;
        public bool IsEncrypting { get; private set; } = false;
        public string Name { get; private set; } = nameof(Name);
        public string ServerName { get; private set; } = nameof(ServerName);

        private bool ListenersOutputMuteStatus { get; set; } = false;
        private bool ListenersInputMuteStatus { get; set; } = false;
        private bool SpeakersOutputMuteStatus { get; set; } = false;
        private bool SpeakersInputMuteStatus { get; set; } = false;
        private bool ModeratorsOutputMuteStatus { get; set; } = false;
        private bool ModeratorsInputMuteStatus { get; set; } = false;

        public Client Server { get; set; } = new Client(new IPEndPoint(IPAddress.Any, 0), 0, nameof(Server), new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second), nameof(Name), nameof(ServerName), false);

        public Session(int Port, string Name, string Password, bool StartServerAnyway = false)
        {
            Manage.Logger.Add("The server is starting...", LogType.Server, LogLevel.Debug);
            this.Port = Port;
            this.Password = Password;
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
                    client.ConnectionInfo.InputClientMuteStatus = clientsInputMuteStatusChangedEvent.InputMuteStatus;
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
                    client.ConnectionInfo.OutputClientMuteStatus = clientsOutputMuteStatusChangedEvent.OutputMuteStatus;
                }
            }
        }
        #endregion

        #region Methods
        public void DragDropSetMuteStatus(int clientId)
        {
            if (Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId) != default)
            {
                switch (Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.ClientStatus)
                {
                    case ClientStatus.Listener:
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetInputServerMuteStatus(ListenersInputMuteStatus);
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetOutputServerMuteStatus(ListenersOutputMuteStatus);
                        break;
                    case ClientStatus.Speaker:
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetInputServerMuteStatus(SpeakersInputMuteStatus);
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetOutputServerMuteStatus(SpeakersOutputMuteStatus);
                        break;
                    case ClientStatus.Moderator:
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetInputServerMuteStatus(ModeratorsInputMuteStatus);
                        Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetOutputServerMuteStatus(ModeratorsOutputMuteStatus);
                        break;
                }
            }
        }
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
            Blacklist.Clear();
            ListeningSocket.Close();
            ListeningSocket.Dispose();
            if (ShouldRedirectPort)
                CloseServerPort(Port, Manage.DefaultInformation.Protocol);
            Server.Record.Save();
            Manage.Logger.Add($"The server closed by {reason}", LogType.Server, LogLevel.Info);
            Manage.ServerSession = null;
        }
        private void SendData(byte[] data, IPEndPoint socket)
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
                Manage.Logger.Add($"Send {nameof(data)} {Manage.GetUlongFromBuffer(data)} to the client {socket}", LogType.Server, LogLevel.Trace);
            }
            catch (SocketException ex)
            {
                Manage.Logger.Add($"Catch {nameof(SocketException)} {ex.Message} during send {nameof(data)} {Manage.GetUlongFromBuffer(data)} to the client {socket}", LogType.Server, LogLevel.Error);
            }
        }
        #endregion

        #region Helper
        public void SetEncrypting(bool value)
        {
            IsEncrypting = value;
        }
        private bool IsKey(byte[] data)
        {
            if (Password.Length > data.Length)
                return false;
            for (int i = 0; i < Password.Length; i++)
            {
                if (Password[i] != data[i])
                    return false;
            }
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
        public void SetClientOutputMuteStatus(int clientId, bool outputMuteStatus)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetOutputServerMuteStatus(outputMuteStatus);
        }

        public void SetClientInputMuteStatus(int clientId, bool inputMuteStatus)
        {
            Clients.FirstOrDefault(x => x.ConnectionInfo.Id == clientId).ConnectionInfo.SetInputServerMuteStatus(inputMuteStatus);
        }


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
            SendData(Encoding.ASCII.GetBytes(Manage.DefaultInformation.DisconnectMessage + reason), client.Socket);
        }
        private void ConnectClient(IPEndPoint iPEndPoint)
        {
            Client client = new Client(iPEndPoint, Clients.Count + DisconnectedClients.Count, "Username", Server.ConnectionInfo.SessionStartTimeSpan, Name, ServerName);
            Clients.Add(client);
            SendData(Encoding.ASCII.GetBytes(Password), client.Socket);
            Manage.Logger.Add($"The user {client.Socket} has connected to the server. Create new {nameof(client.ConnectionInfo.Key)} {Manage.GetStringFromBuffer(client.ConnectionInfo.Key())}", LogType.Server, LogLevel.Info);

            new Thread(delegate ()
            {
                CheckVerification(client);
            }).Start();
        }
        private void BlockClient(IPEndPoint client, string reason)
        {
            Blacklist.Add(client.Address);
            SendData(new byte[Manage.DefaultInformation.DataLength], client);
            Manage.Logger.Add($"The user {client} was refused to connect and was added to the blacklist by {reason}", LogType.Server, LogLevel.Warn);
        }
        private void AddAudio(byte[] data, Client sender)
        {
            if (sender.ConnectionInfo.InputServerMuteStatus)
                return;
            if (Server.Record.IsRecording)
            {
                Server.AddAudio(data);
            }
            foreach (Client client in Clients)
            {
                if (client.ConnectionInfo.OutputServerMuteStatus || (sender.Socket.Address.Equals(client.Socket.Address) && sender.Socket.Port == client.Socket.Port))
                    continue;
                client.AddAudio(data);
            }
        }
        #endregion

        #region Threads
        private void SendingData()
        {
            while (ServerStage == ServerStage.Open)
            {
                foreach (Client client in Clients.ToList())
                {
                    byte[] data = client.GetAudio();
                    if (Manage.GetUlongFromBuffer(data) > 0)
                        SendData(data, client.Socket);
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
                        SendData(client.ConnectionInfo.Key(), client.Socket);
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
                if (Blacklist.Contains(iPEndPoint.Address))
                    continue;
                Client client = GetClientBySocket(iPEndPoint);
                if (client != null)
                {
                    if (client.ConnectionInfo.IsVerificationMessage(data))
                    {
                        client.UpdateVerification(data);
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
                        BlockClient(iPEndPoint, $"by wrong {nameof(Password)} {Manage.GetStringFromBuffer(data)}");
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