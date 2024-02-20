using Core.Main;
using Core.Server;
using System;
using System.ComponentModel;

namespace Core.Client
{
    public class ClientInfo : INotifyPropertyChanged
    {
        public int Id { get; set; } = 0;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                NotifyPropertyChanged(nameof(Username));
            }
        }
        private string username { get; set; } = string.Empty;
        public ClientStatus ClientStatus
        {
            get { return clientStatus; }
            set
            {
                clientStatus = value;
                NotifyPropertyChanged(nameof(ClientStatus));
            }
        }
        private ClientStatus clientStatus { get; set; } = ClientStatus.Listener;
        public TimeSpan ConnectionTimeSpan
        {
            get { return connectionTimeSpan; }
            set
            {
                connectionTimeSpan = value;
                NotifyPropertyChanged(nameof(ConnectionTimeSpan));
            }
        }
        private TimeSpan connectionTimeSpan { get; set; } = new TimeSpan(0, 0, 0);


        public ClientInfo(int Id, string Username, ClientStatus ClientStatus, TimeSpan ConnectionTimeSpan)
        {
            this.Id = Id;
            this.Username = Username;
            this.ClientStatus = ClientStatus;
            this.ConnectionTimeSpan = ConnectionTimeSpan;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged.Raise(this, info);
        }
        public void CompareToNew(ClientInfo clientInfo)
        {
            Username = clientInfo.Username;
            ClientStatus = clientInfo.ClientStatus;
            ConnectionTimeSpan = clientInfo.ConnectionTimeSpan;
        }
    }
}