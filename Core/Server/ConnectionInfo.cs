using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Main;

namespace Core.Server
{
    public class ConnectionInfo
    {

        #region static
        public bool IsVerified { get; set; } = false;
        public int Id { get; private set; } = 0;
        private DateTime ConnectDateTime { get; set; } = DateTime.Now;
        public string VerificationMessage { get; private set; } = Manage.DefaultInformation.VerificationMessage;
        #endregion

        #region client decision
        public bool OutputClientMuteStatus { get; set; } = false;
        public bool InputClientMuteStatus { get; set; } = false;
        public string Username { get; private set; } = string.Empty;
        #endregion

        #region server decision
        public bool OutputServerMuteStatus { get; private set; } = false;
        public bool InputServerMuteStatus { get; private set; } = false;
        public ClientStatus ClientStatus { get; private set; } = ClientStatus.Listener;
        public string SessionName { get; private set; } = string.Empty;
        public string ServerName { get; private set; } = string.Empty;
        public TimeSpan SessionStartTimeSpan { get; private set; } = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        #endregion
        public TimeSpan ConnectionTimeSpan
        {
            get { return ConnectionDateTime(DateTime.Now); }
        }
        public object[] Data { get; private set; } = new object[] { };

        public ConnectionInfo(int Id, string Username, TimeSpan SessionStartTimeSpan, string VerificationMessage = "", string SessionName = "", string ServerName = "")
        {
            this.Id = Id;
            this.Username = Username;
            this.SessionStartTimeSpan = SessionStartTimeSpan;
            this.VerificationMessage = VerificationMessage;
            this.SessionName = SessionName;
            this.ServerName = ServerName;
            UpdateData();
        }

        public void SetOutputServerMuteStatus(bool OutputServerMuteStatus)
        {
            this.OutputServerMuteStatus = OutputServerMuteStatus;
            Manage.Logger.Add($"Set {nameof(OutputServerMuteStatus)} to {OutputServerMuteStatus} for {Id}", LogType.Server, LogLevel.Debug);
        }
        public void SetInputServerMuteStatus(bool InputServerMuteStatus)
        {
            this.InputServerMuteStatus = InputServerMuteStatus;
            Manage.Logger.Add($"Set {nameof(InputServerMuteStatus)} to {InputServerMuteStatus} for {Id}", LogType.Server, LogLevel.Debug);
        }


        public void SetClientStatus(ClientStatus ClientStatus)
        {
            this.ClientStatus = ClientStatus;
        }

        private TimeSpan ConnectionDateTime(DateTime dateTime)
        {
            TimeSpan timeSpan = dateTime - ConnectDateTime;
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private bool IsVerificationData(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.ASCII.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (temp.Length != Data.Length)
                return false;
            return true;
        }

        public bool IsVerificationMessage(byte[] data)
        {
            return IsVerificationData(data) && Encoding.ASCII.GetString(data).Contains(VerificationMessage);
        }

        private void UpdateData()
        {
            Data = new object[]
            {
                Username, OutputServerMuteStatus, InputServerMuteStatus, OutputClientMuteStatus, InputClientMuteStatus,
                SessionName, ClientStatus, VerificationMessage, ServerName, Id, SessionStartTimeSpan
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
            return Encoding.ASCII.GetBytes(key);
        }

        public void DecomposeClient(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.ASCII.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (bool.TryParse(temp[1].ToString(), out bool OutputServerMuteStatus))
                this.OutputServerMuteStatus = OutputServerMuteStatus;
            if (bool.TryParse(temp[2].ToString(), out bool InputServerMuteStatus))
                this.InputServerMuteStatus = InputServerMuteStatus;
            if (temp[5].ToString() != null && temp[5].ToString().Length > 0)
                SessionName = temp[5].ToString();
            if (Enum.TryParse(temp[6].ToString(), out ClientStatus ClientStatus))
                this.ClientStatus = ClientStatus;
            if (temp[7].ToString() != null && temp[7].ToString().Length > 0)
                VerificationMessage = temp[7].ToString();
            if (temp[8].ToString() != null && temp[8].ToString().Length > 0)
                ServerName = temp[8].ToString();
            if (temp[10].ToString() != null && TimeSpan.TryParse(temp[10].ToString(), out TimeSpan SessionStartTimeSpan))
                this.SessionStartTimeSpan = SessionStartTimeSpan;
            UpdateData();
        }

        public void DecomposeServer(byte[] data)
        {
            object[] temp = new Regex(@"\W\B").Replace(Encoding.ASCII.GetString(data), string.Empty).Split(Manage.DefaultInformation.SplitSymbol);
            if (temp[0].ToString() != null && temp[0].ToString().Length > 0)
                Username = temp[0].ToString();
            if (bool.TryParse(temp[3].ToString(), out bool OutputClientMuteStatus))
                this.OutputClientMuteStatus = OutputClientMuteStatus;
            if (bool.TryParse(temp[4].ToString(), out bool InputClientMuteStatus))
                this.InputClientMuteStatus = InputClientMuteStatus;
            UpdateData();
        }
    }
}