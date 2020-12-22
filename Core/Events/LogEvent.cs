using Core.Handlers;
using Core.Main;

namespace Core.Events
{
    public class LogEvent : Event
    {
        public string Message { get; set; } = string.Empty;
        public LogType LogType { get; set; } = LogType.Application;
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;
        public LogEvent(string Message, LogType LogType, LogLevel LogLevel)
        {
            this.Message = Message;
            this.LogType = LogType;
            this.LogLevel = LogLevel;
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerLog)handler).OnLog(this);
        }
    }
}