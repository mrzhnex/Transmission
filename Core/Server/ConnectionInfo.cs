using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Core.Main;

namespace Core.Server
{
    public class ConnectionInfo : INotifyPropertyChanged
    {
        public static ConnectionInfo Default = new ConnectionInfo(-1, "Default", TimeSpan.Zero, IPAddress.Loopback);

        #region static
        public bool IsVerified { get; set; } = false;
        public int Id { get; private set; } = 0;
        private DateTime ConnectDateTime { get; set; } = DateTime.Now;
        public string VerificationMessage { get; private set; } = Manage.DefaultInformation.VerificationMessage;
        public string Ip { get; set; } = string.Empty;
        #endregion

        #region client decision
        public bool OutputMuteStatus { get; set; } = false;
        public bool InputMuteStatus { get; set; } = false;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        private string username = string.Empty;
        #endregion

        #region server decision
        public float ServerOutputVolumeValue
        {
            get { return serverOutputVolumeValue; }
            set
            {
                serverOutputVolumeValue = value;
                OnPropertyChanged(nameof(ServerOutputVolumeValue));
            }
        }
        private float serverOutputVolumeValue { get; set; } = 1.0f;
        public float ServerInputVolumeValue
        {
            get { return serverInputVolumeValue; }
            set
            {
                serverInputVolumeValue = value;
                OnPropertyChanged(nameof(ServerInputVolumeValue));
            }
        }
        private float serverInputVolumeValue { get; set; } = 1.0f;
        public ClientStatus ClientStatus
        {
            get { return clientStatus; }
            set
            {
                clientStatus = value;
                OnPropertyChanged(nameof(ClientStatus));
            }
        }
        private ClientStatus clientStatus = ClientStatus.Listener;
        public string SessionName { get; private set; } = string.Empty;
        public string ServerName { get; private set; } = string.Empty;
        public TimeSpan SessionStartTimeSpan { get; private set; } = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        public bool ServerOutputMuteStatus { get; set; } = false;
        public bool ServerInputMuteStatus { get; set; } = false;
        #endregion
        public TimeSpan ConnectionTimeSpan
        {
            get { return connectionTimeSpan; }
            set
            {
                connectionTimeSpan = value;
                OnPropertyChanged(nameof(ConnectionTimeSpan));
            }
        }
        private TimeSpan connectionTimeSpan = TimeSpan.Zero;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public object[] Data { get; private set; } = new object[] { };

        public ConnectionInfo(int Id, string Username, TimeSpan SessionStartTimeSpan, IPAddress iPAddress, string VerificationMessage = "", string SessionName = "SessionName", string ServerName = "ServerName")
        {
            this.Id = Id;
            this.Username = Username;
            this.SessionStartTimeSpan = SessionStartTimeSpan;
            this.VerificationMessage = VerificationMessage;
            this.SessionName = SessionName;
            this.ServerName = ServerName;
            Ip = iPAddress.ToString();
            ConnectionTimeSpan = new TimeSpan(0, 0, 0);
            UpdateData();
        }

        public ConnectionInfo() { }

        public void SetClientStatus(ClientStatus ClientStatus)
        {
            this.ClientStatus = ClientStatus;
        }

        public void UpdateConnectionTimeSpan()
        {
            ConnectionTimeSpan = ConnectionDateTime(DateTime.Now);
        }

        private TimeSpan ConnectionDateTime(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - ConnectDateTime;
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private bool IsVerificationData(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.UTF8.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (temp.Length != Data.Length)
                return false;
            return true;
        }

        public bool IsVerificationMessage(byte[] data)
        {
            return IsVerificationData(data) && Encoding.UTF8.GetString(data).Contains(VerificationMessage);
        }

        private void UpdateData()
        {
            Data = new object[]
            {
                Username, OutputMuteStatus, InputMuteStatus, SessionName, 
                ClientStatus, VerificationMessage, ServerName, Id, SessionStartTimeSpan
            };
        }

        public byte[] Key()
        {
            string key = string.Empty;
            for (int i = 0; i < Data.Length; i++)
            {
                if (i == 0)
                    key += Data[i];
                else
                    key = key + Manage.DefaultInformation.SplitSymbol + Data[i];
            }
            return Encoding.UTF8.GetBytes(key);
        }

        public void DecomposeClient(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.UTF8.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (temp[3].ToString() != null && temp[5].ToString().Length > 0)
                SessionName = temp[3].ToString();
            if (Enum.TryParse(temp[4].ToString(), out ClientStatus ClientStatus))
                this.ClientStatus = ClientStatus;
            if (temp[5].ToString() != null && temp[5].ToString().Length > 0)
                VerificationMessage = temp[5].ToString();
            if (temp[6].ToString() != null && temp[6].ToString().Length > 0)
                ServerName = temp[6].ToString();
            if (temp[8].ToString() != null && TimeSpan.TryParse(temp[8].ToString(), out TimeSpan SessionStartTimeSpan))
                this.SessionStartTimeSpan = SessionStartTimeSpan;
            UpdateData();
        }

        public void DecomposeServer(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.UTF8.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (temp[0].ToString() != null && temp[0].ToString().Length > 0)
                Username = temp[0].ToString();
            if (bool.TryParse(temp[1].ToString(), out bool OutputMuteStatus))
                this.OutputMuteStatus = OutputMuteStatus;
            if (bool.TryParse(temp[2].ToString(), out bool InputMuteStatus))
                this.InputMuteStatus = InputMuteStatus;
            UpdateData();
        }
    }
}