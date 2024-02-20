using Core.Events;
using Core.Handlers;
using Core.Main;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ClientSettings : Settings
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

        private XmlSerializer XmlSerializer { get; set; } = new XmlSerializer(typeof(ClientSettings));
        public void Load()
        {
            if (File.Exists(Info.ClientSettingsFullFileName()))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(Info.ClientSettingsFullFileName(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        Manage.ApplicationManager.ClientSettings = (ClientSettings)XmlSerializer.Deserialize(fileStream);
                    }
                    Manage.Logger.Add($"The application settings have been downloaded", LogType.Application, LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    Manage.Logger.Add($"Catch an exception {ex.Message} during application download", LogType.Application, LogLevel.Error);
                    CreateNewAndSave();
                }
            }
            else
            {
                CreateNewAndSave();
            }
            Manage.EventManager.ExecuteEvent<IEventHandlerSettingsLoaded>(new SettingsLoadedEvent(Manage.ApplicationManager.ClientSettings));
        }

        public void Save()
        {
            Manage.Logger.Add($"Trying to save application settings", LogType.Application, LogLevel.Debug);
            if (File.Exists(Info.ClientSettingsFullFileName()))
            {
                File.Delete(Info.ClientSettingsFullFileName());
            }
            using (FileStream fileStream = new FileStream(Info.ClientSettingsFullFileName(), FileMode.Create))
            {
                XmlSerializer.Serialize(fileStream, Manage.ApplicationManager.ClientSettings);
            }
            Manage.Logger.Add($"The application settings were saved", LogType.Application, LogLevel.Info);
        }

        private void CreateNewAndSave()
        {
            Manage.Logger.Add($"Create default application settings", LogType.Application, LogLevel.Debug);
            Save();
        }
    }
}