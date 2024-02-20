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
    public class ServerSettings : Settings
    {
        public int RecordSaveTime { get; set; } = 10;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                if (Manage.ServerSession != null)
                    Manage.ServerSession.SetPassword(value);
            }
        }
        [XmlIgnore]
        private string password { get; set; } = string.Empty;
        public int Port { get; set; } = 5555;
        public string SaveFolder { get; set; } = string.Empty;
        public string RecordSaveFolder { get; set; } = string.Empty;
        public string PlayAudioFile { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public bool ShouldMirrorAudio
        {
            get { return shouldMirrorAudio; }
            set
            {
                Manage.Logger.Add($"Change {nameof(ShouldMirrorAudio)} from {shouldMirrorAudio} to {value}", LogType.Application, LogLevel.Debug);
                shouldMirrorAudio = value;
            }
        }
        [XmlIgnore]
        private bool shouldMirrorAudio { get; set; } = false;
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
        private ThemeType themeType { get; set; } = ThemeType.Default;
        [XmlIgnore]
        public static Theme Theme { get; set; } = Theme.Default;

        public ServerSettings() { }

        private XmlSerializer XmlSerializer { get; set; } = new XmlSerializer(typeof(ServerSettings));
        public void Load()
        {
            if (File.Exists(Info.ServerSettingsFullFileName()))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(Info.ServerSettingsFullFileName(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        Manage.ApplicationManager.ServerSettings = (ServerSettings)XmlSerializer.Deserialize(fileStream);
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
            Manage.EventManager.ExecuteEvent<IEventHandlerSettingsLoaded>(new SettingsLoadedEvent(Manage.ApplicationManager.ServerSettings));
        }

        public void Save()
        {
            Manage.Logger.Add($"Trying to save application settings", LogType.Application, LogLevel.Debug);
            if (File.Exists(Info.ServerSettingsFullFileName()))
            {
                File.Delete(Info.ServerSettingsFullFileName());
            }
            using (FileStream fileStream = new FileStream(Info.ServerSettingsFullFileName(), FileMode.Create))
            {
                XmlSerializer.Serialize(fileStream, Manage.ApplicationManager.ServerSettings);
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