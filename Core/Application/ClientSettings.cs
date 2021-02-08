using Core.Main;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ClientSettings : INotifyPropertyChanged
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
        public ThemeType ThemeType
        {
            get { return themeType; }
            set
            {
                Manage.Logger.Add($"Change {nameof(ThemeType)} from {themeType} to {value}", LogType.Application, LogLevel.Debug);
                themeType = value;
                OnPropertyChanged(nameof(ThemeType));
                Theme.Change(Manager.Themes[value]);
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
        public  ClientSettings() { }
    }
}