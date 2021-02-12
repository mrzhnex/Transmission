using Core.Events;
using Core.Handlers;
using Core.Main;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ClientSettings
    {
        public string ClientName { get; set; } = "имя";
        public int RecordSaveTime { get; set; } = 10;
        public bool OutputMuteStatus { get; set; } = false;
        public bool InputMuteStatus { get; set; } = false;
        public float InputVolumeValue { get; set; } = 1.0f;
        public float OutputVolumeValue { get; set; } = 1.0f;
        public string RecordSaveFolder { get; set; } = string.Empty;
        public string PlayAudioFile { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public bool ShouldLog
        {
            get { return shouldLog; }
            set
            {
                Manage.Logger.Add($"Change {nameof(ShouldLog)} from {shouldLog} to {value}", LogType.Application, LogLevel.Debug);
                shouldLog = value;
                Manage.EventManager.ExecuteEvent<IEventHandlerShouldLogChanged>(new ShouldLogChangedEvent(value));
            }
        }
        public ThemeType ThemeType
        {
            get { return themeType; }
            set
            {
                Manage.Logger.Add($"Change {nameof(ThemeType)} from {themeType} to {value}", LogType.Application, LogLevel.Debug);
                themeType = value;
                Theme.Change(Manager.Themes[Manager.Themes.Keys.FirstOrDefault(x => x.ThemeType == value)]);
            }
        }

        [XmlIgnore]
        private bool shouldLog { get; set; } = true;
        [XmlIgnore]
        private ThemeType themeType { get; set; } = ThemeType.Default;
        [XmlIgnore]
        public static Theme Theme { get; set; } = Theme.Default;
        public ClientSettings() { }
    }
}