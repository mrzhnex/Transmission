using Core.Main;
using Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Client
{
    public class ClientInfoBehaviour
    {
        #region Helper
        private string GetStringFromClientInfo(ClientInfo clientInfo)
        {
            return clientInfo.Id.ToString() + "#" + clientInfo.Username + "#" + clientInfo.ClientStatus + "#" + clientInfo.ConnectionTimeSpan;
        }
        private ClientInfo GetClientInfoFromString(string result)
        {
            string[] data = result.Split('#');
            return new ClientInfo(int.Parse(data[0]), data[1], (ClientStatus)Enum.Parse(typeof(ClientStatus), data[2]), TimeSpan.Parse(data[3]));
        }
        private string[] GetClientInfoStringsFromData(byte[] data)
        {
            List<string> result = Encoding.UTF8.GetString(data).Split(';').ToList();
            result.RemoveAt(0);
            result.RemoveAt(result.Count - 1);
            return result.ToArray();
        }
        #endregion

        public List<ClientInfo> GetClientInfosFromData(byte[] data)
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            foreach (string s in GetClientInfoStringsFromData(data))
            {
                clientInfos.Add(GetClientInfoFromString(s));
            }
            return clientInfos;
        }
        public bool IsClientInfosMessage(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Contains(Manage.DefaultInformation.ClientInfoMessage);
        }
        public byte[] GetClientInfosData(List<Server.Client> clients)
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            foreach (Server.Client client in clients)
            {
                clientInfos.Add(new ClientInfo(client.ConnectionInfo.Id, client.ConnectionInfo.Username, client.ConnectionInfo.ClientStatus, client.ConnectionInfo.ConnectionTimeSpan));
            }
            string result = Manage.DefaultInformation.ClientInfoMessage + ";";
            foreach (ClientInfo clientInfo in clientInfos)
            {
                result = result + GetStringFromClientInfo(clientInfo) + ";";
            }
            return Encoding.UTF8.GetBytes(result);
        }
        public byte[] GetClientInfosData(ClientInfo clientInfo)
        {
            return Encoding.UTF8.GetBytes(Manage.DefaultInformation.ClientInfoMessage + ";" + GetStringFromClientInfo(clientInfo) + ";");
        }
    }
}