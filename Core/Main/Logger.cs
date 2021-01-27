using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Core.Events;
using Core.Handlers;

namespace Core.Main
{
    public class Logger
    {
        #region Main
        private Thread LogThread { get; set; }
        private List<string> ApplicationLogs { get; set; } = new List<string>();
        private List<string> ClientLogs { get; set; } = new List<string>();
        private List<string> ServerLogs { get; set; } = new List<string>();
        public bool ActiveLog { get; set; } = true;
        public bool ShouldLog { get; set; } = true;
        #endregion

        internal Logger()
        {
            CreateDirectories();
            LogThread = new Thread(LogMethod);
            LogThread.Start();
        }
        ~Logger()
        {
            ActiveLog = false;
        }

        #region Data
        private string ApplicationLogsFileName { get; set; } = "ApplicationLog.txt";
        private string ClientLogsFileName { get; set; } = "ClientLog.txt";
        private string ServerLogsFileName { get; set; } = "ServerLog.txt";
        public string LogsFolder { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Manage.DefaultInformation.ApplicationName, "Logs", $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}");
        #endregion

        #region Save Methods
        private void SaveApplicationLogs()
        {
            List<string> TempLogs = new List<string>();
            lock (ApplicationLogs)
                TempLogs.AddRange(ApplicationLogs);
            if (TempLogs.Count > 0)
                File.AppendAllLines(GetApplicationLogsFullFileName(), TempLogs, Encoding.UTF8);
            ApplicationLogs.RemoveRange(0, TempLogs.Count);
        }
        private void SaveServerLogs()
        {
            List<string> TempLogs = new List<string>();
            lock (ServerLogs)
                TempLogs.AddRange(ServerLogs);
            if (TempLogs.Count > 0)
                File.AppendAllLines(GetServerLogsFullFileName(), TempLogs, Encoding.UTF8);
            ServerLogs.RemoveRange(0, TempLogs.Count);
        }
        private void SaveClientLogs()
        {
            List<string> TempLogs = new List<string>();
            lock (ClientLogs)
                TempLogs.AddRange(ClientLogs);
            if (TempLogs.Count > 0)
                File.AppendAllLines(GetClientLogsFullFileName(), TempLogs, Encoding.UTF8);
            ClientLogs.RemoveRange(0, TempLogs.Count);
        }
        internal void SaveAllLogs()
        {
            if (!ShouldLog)
                return;
            if (!IsUsedByAnotherProcess(GetApplicationLogsFullFileName()))
                SaveApplicationLogs();
            if (!IsUsedByAnotherProcess(GetServerLogsFullFileName()))
                SaveServerLogs();
            if (!IsUsedByAnotherProcess(GetClientLogsFullFileName()))
                SaveClientLogs();
        }
        private void LogMethod()
        {
            while (ActiveLog)
            {
                SaveAllLogs();
                Thread.Sleep(5000);
            }
        }
        #endregion

        #region Helper
        private void CreateDirectories()
        {
            if (!Directory.Exists(LogsFolder))
                Directory.CreateDirectory(LogsFolder);
            if (!File.Exists(GetApplicationLogsFullFileName()))
                File.Create(GetApplicationLogsFullFileName()).Close();
            if (!File.Exists(GetServerLogsFullFileName()))
                File.Create(GetServerLogsFullFileName()).Close();
            if (!File.Exists(GetClientLogsFullFileName()))
                File.Create(GetClientLogsFullFileName()).Close();
        }
        private bool IsUsedByAnotherProcess(string filePath)
        {
            try { using (Stream stream = new FileStream(filePath, FileMode.Open)) { } }
            catch { return true; }
            return false;
        }
        private string GetApplicationLogsFullFileName()
        {
            return Path.Combine(LogsFolder, ApplicationLogsFileName);
        }
        private string GetClientLogsFullFileName()
        {
            return Path.Combine(LogsFolder, ClientLogsFileName);
        }
        private string GetServerLogsFullFileName()
        {
            return Path.Combine(LogsFolder, ServerLogsFileName);
        }
        private string ConstructStringLog(string message, LogLevel logLevel)
        {
            return $"[{logLevel}][{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}]:{message}";
        }
        #endregion

        public void Add(string message, LogType logType, LogLevel logLevel)
        {
            Manage.EventManager.ExecuteEvent<IEventHandlerLog>(new LogEvent(message, logType, logLevel));
            if (!ShouldLog)
                return;
            switch (logType)
            {
                default:
                case LogType.Application:
                    lock (ClientLogs)
                        ApplicationLogs.Add(ConstructStringLog(message, logLevel));
                    break;
                case LogType.Client:
                    lock (ClientLogs)
                        ClientLogs.Add(ConstructStringLog(message, logLevel));
                    break;
                case LogType.Server:
                    lock (ClientLogs)
                        ServerLogs.Add(ConstructStringLog(message, logLevel));
                    break;
            }
        }
    }

    public enum LogType
    {
        Client, Server, Application
    }
    public enum LogLevel
    {
        Trace, Debug, Info, Warn, Error, Fatal
    }
}