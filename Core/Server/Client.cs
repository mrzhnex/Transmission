using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Core.Application;
using Core.Main;

namespace Core.Server
{
    public class Client
    {
        public VerificationStage Stage { get; private set; } = VerificationStage.Connected;
        public IPEndPoint Socket { get; private set; } = new IPEndPoint(IPAddress.Any, 0);
        private List<byte> AudioData { get; set; } = new List<byte>();
        public ConnectionInfo ConnectionInfo { get; private set; }
        public Record Record { get; private set; }

        public Client(IPEndPoint Socket, int Id, string Username, TimeSpan SessionStartTimeSpan, string SessionName, string ServerName, bool IsClient, bool IsRecording)
        {
            this.Socket = Socket;
            Record = new Record(SessionName, this, IsClient, IsRecording);
            ConnectionInfo = new ConnectionInfo(Id, Username, SessionStartTimeSpan, Socket.Address, Manage.DefaultInformation.VerificationMessage + Id.ToString(), SessionName, ServerName);
        }

        public void AddAudio(byte[] data)
        {
            Task.Run(new Action(() => AddAudioCore(data)));
        }

        private void AddAudioCore(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                lock (AudioData)
                {
                    if (AudioData.Count > i)
                    {
                        AudioData[i] = (byte)(AudioData[i] + data[i]);
                    }
                    else
                    {
                        AudioData.Add(data[i]);
                    }
                }
            }
        }

        public byte[] GetAudio()
        {
            byte[] data = new byte[Manage.DefaultInformation.DataLength];
            int length = AudioData.Count < data.Length ? AudioData.Count : data.Length;
            for (int i = 0; i < length; i++)
            {
                data[i] = AudioData[i];
            }
            lock (AudioData)
            {
                AudioData.RemoveRange(0, length);
            }
            return data;
        }

        #region Verification
        public void UpdateVerification(byte[] data)
        {
            if (Stage == VerificationStage.Disconnected)
                return;
            ConfirmVerification(data);
            Manage.Logger.Add($"Setting the {nameof(VerificationStage)} to {Stage} for the client {Socket} with {nameof(ConnectionInfo.ConnectionTimeSpan)} {ConnectionInfo.ConnectionTimeSpan}", LogType.Server, LogLevel.Trace);
        }
        private void ConfirmVerification(byte[] data)
        {
            ConnectionInfo.IsVerified = true;
            Stage = VerificationStage.Connected;
            ConnectionInfo.DecomposeServer(data);
        }
        public void DeniedVerification()
        {
            switch (Stage)
            {
                case VerificationStage.Connected:
                    Stage = VerificationStage.Validated;
                    break;
                case VerificationStage.Validated:
                    Stage = VerificationStage.NonValidated;
                    break;
                case VerificationStage.NonValidated:
                    Stage = VerificationStage.Disconnected;
                    break;
            }
        }
        #endregion
    }
    public enum VerificationStage
    {
        Connected, Validated, NonValidated, Disconnected
    }
    public enum ClientStatus
    {
        Moderator, Speaker, Listener
    }
}