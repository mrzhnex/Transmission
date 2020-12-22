using Core.Events;
using Core.Handlers;
using Core.Main;
using System;
using System.Windows.Threading;

namespace Server
{
    public class Application : Core.Main.Application, IEventHandlerLog, IEventHandlerServerUpdate
    {

        public void OnServerUpdate(ServerUpdateEvent serverUpdateEvent)
        {
            MainWindow.MainWindowInstance.UpdateServerInfo(serverUpdateEvent.Clients);
            MainWindow.MainWindowInstance.Time.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { MainWindow.MainWindowInstance.Time.Text = $"{Manage.ServerSession.Server.ConnectionInfo.SessionStartTimeSpan} - {new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)}"; }));
        }

        public void OnLog(LogEvent logEvent)
        {
            if (logEvent.LogLevel < LogLevel.Info)
                return;
        }

        protected override bool CanInput()
        {
            return true;
        }
        protected override bool CanOutput()
        {
            return true;
        }
        protected override void PrepareInput() { }
        protected override void PrepareOutput() { }
    }
}