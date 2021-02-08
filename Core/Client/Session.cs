using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Core.Events;
using Core.Handlers;
using Core.Main;
using Core.Server;

namespace Core.Client
{
    public class Session : IEventHandlerInput
    {
        private ClientStage ClientStage { get; set; } = ClientStage.Starting;
        public IPEndPoint ServerIpEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 0);
        private Thread ConnectionThread { get; set; }
        private Thread ConnectedThread { get; set; }
        private Thread CheckConnectThread { get; set; }
        private Socket ListeningSocket { get; set; } = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private ConnectionInfo ConnectionInfo { get; set; }
        private int ConnectTimeout { get; set; } = 3000;
        private string ServerKey { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public TimeSpan ConnectTimeSpan { get; set; } = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        public Session(IPAddress iPAddress, string Username, int port, string Serverkey)
        {
            Manage.EventManager.AddEventHandlers(this);
            ServerKey = Serverkey;
            this.Username = Username;

            ConnectedThread = new Thread(Connected);
            ConnectionThread = new Thread(Connection);
            CheckConnectThread = new Thread(CheckConnect);
            ServerIpEndPoint = new IPEndPoint(iPAddress, port);
            ListeningSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

            ClientStage = ClientStage.Connection;
            ConnectedThread.Start();
            ConnectionThread.Start();
            CheckConnectThread.Start();
            SendData(Manage.GetDataFromString(Serverkey));
            Manage.Logger.Add($"Send {nameof(ServerKey)} {ServerKey} to the server {ServerIpEndPoint}", LogType.Client, LogLevel.Info);
        }
        
        public void SetUsername(string Username)
        {
            this.Username = Username;
            ConnectionInfo.Username = Username;
        }

        public void OnInput(InputEvent inputEvent)
        {
            if (ClientStage == ClientStage.Connected)
            {
                Manage.Logger.Add($"Send {nameof(inputEvent.Data)} {Manage.GetUlongFromBuffer(inputEvent.Data)} to the server {ServerIpEndPoint}", LogType.Client, LogLevel.Trace);
                SendData(inputEvent.Data);
            }
        }

        #region Methods
        private void SendData(byte[] data)
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
            ListeningSocket.SendTo(data, ServerIpEndPoint);
        }
        private void Connect(string ip, string key)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerConnect>(new ConnectEvent(ip));
            Manage.Logger.Add($"The server {ServerIpEndPoint} allowed the connection by {key}", LogType.Client, LogLevel.Info);
            ConnectTimeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            ConnectionInfo = new ConnectionInfo(0, Username, ConnectTimeSpan, IPAddress.Loopback);
            ClientStage = ClientStage.Connected;
        }
        public void Disconnect(string reason)
        {
            if (ClientStage == ClientStage.Disconnection)
                return;
            ClientStage = ClientStage.Disconnection;
            reason = new Regex(@"\W\B").Replace(reason, string.Empty);
            Manage.Logger.Add("Start disconnection process...", LogType.Client, LogLevel.Debug);
            Manage.EventManager.ExecuteEvent<IEventHandlerDisconnect>(new DisconnectEvent(reason));
            ListeningSocket.Close();
            ListeningSocket.Dispose();
            Manage.Logger.Add($"Disconnected by {reason}", LogType.Client, LogLevel.Info);
            Manage.ClientSession = null;
        }
        #endregion

        #region Threads
        private void CheckConnect()
        {
            Thread.Sleep(ConnectTimeout);
            if (ClientStage == ClientStage.Connection)
            {
                Disconnect("Timeout");
            }
        }
        private void Connection()
        {
            EndPoint remoteIp = new IPEndPoint(ServerIpEndPoint.Address, ServerIpEndPoint.Port);
            while (ClientStage == ClientStage.Connection)
            {
                if (ListeningSocket.Available > 0)
                {
                    byte[] data = new byte[Manage.DefaultInformation.DataLength];
                    try
                    {
                        ListeningSocket.ReceiveFrom(data, ref remoteIp);
                    }
                    catch (SocketException ex)
                    {
                        Manage.Logger.Add($"Catch {nameof(SocketException)} {ex.Message} during reception {Manage.GetUlongFromBuffer(data)} from {remoteIp as IPEndPoint}", LogType.Application, LogLevel.Error);
                        Disconnect(ex.Message);
                        break;
                    }
                    if (Encoding.UTF8.GetString(data).Contains(Manage.DefaultInformation.DisconnectMessage))
                    {
                        Disconnect(Encoding.UTF8.GetString(data).Replace(Manage.DefaultInformation.DisconnectMessage, string.Empty));
                    }
                    else if (Manage.GetStringFromData(Manage.ParseKeyFromString(ServerKey)) == Manage.GetStringFromData(data))
                    {
                        Connect((remoteIp as IPEndPoint).ToString(), Manage.GetStringFromData(data));
                    }
                    else
                    {
                        Disconnect($"The server {ServerIpEndPoint} refused the connection");
                    }
                }
            }
        }
        private void Connected()
        {
            EndPoint remoteIp = new IPEndPoint(ServerIpEndPoint.Address, ServerIpEndPoint.Port);
            while (ClientStage == ClientStage.Connection)
            {
                Thread.Sleep(1);
            }
            while (ClientStage == ClientStage.Connected)
            {
                if (ListeningSocket.Available > 0)
                {
                    byte[] data = new byte[Manage.DefaultInformation.DataLength];
                    try
                    {
                        ListeningSocket.ReceiveFrom(data, ref remoteIp);
                    }
                    catch (SocketException ex)
                    {
                        Manage.Logger.Add($"Catch {nameof(SocketException)} {ex.Message} during reception {Manage.GetUlongFromBuffer(data)} from {remoteIp as IPEndPoint}", LogType.Application, LogLevel.Error);
                        Disconnect(ex.Message);
                        break;
                    }
                    if (Encoding.UTF8.GetString(data).Contains(Manage.DefaultInformation.DisconnectMessage))
                    {
                        Disconnect(Encoding.UTF8.GetString(data).Replace(Manage.DefaultInformation.DisconnectMessage, string.Empty));
                    }
                    else if (ConnectionInfo.IsVerificationMessage(data))
                    {
                        ConnectionInfo.DecomposeClient(data);
                        Manage.Logger.Add($"Send {nameof(ConnectionInfo.Key)} {Manage.GetStringFromData(data)} to the server {ServerIpEndPoint}", LogType.Client, LogLevel.Trace);
                        SendData(ConnectionInfo.Key());
                        Manage.EventManager.ExecuteEvent<IEventHandlerClientUpdate>(new ClientUpdateEvent(ConnectionInfo));
                    }
                    else
                    {
                        Manage.Logger.Add($"Listen {Manage.GetUlongFromBuffer(data)} from {remoteIp as IPEndPoint}", LogType.Client, LogLevel.Trace);
                        Manage.EventManager.ExecuteEvent<IEventHandlerOutput>(new OutputEvent(data));
                    }
                }
            }
        }
        #endregion
    }

    public enum ClientStage
    {
        Starting, Connection, Connected, Disconnection
    }
}