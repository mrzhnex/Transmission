using Core.Main;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ServerSettings : INotifyPropertyChanged
    {
        public int RecordSaveTime { get; set; } = 10;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 5555;
        public string SaveFolder { get; set; } = string.Empty;
        public string RecordSaveFolder { get; set; } = string.Empty;
        public string PlayAudioFile { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public ThemeType ThemeType
        {
            get { return themeType; }
            set
            {
                Manage.Logger.Add($"Change {nameof(ThemeType)} from {themeType} to {value}", LogType.Application, LogLevel.Debug);
                themeType = value;
                OnPropertyChanged(nameof(ThemeType));
                Theme.Change(Manager.Themes[Manager.Themes.Keys.FirstOrDefault(x => x.ThemeType == value)]);
            }
        }

        [XmlIgnore]
        private ThemeType themeType { get; set; } = ThemeType.Default;

        [XmlIgnore]
        public static Theme Theme { get; set; } = Theme.Default;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ServerSettings() { }
    }
}