using Core.Events;
using Core.Handlers;
using Core.Main;
using System.IO;
using System.Xml.Serialization;

namespace Core.Application
{
    public class Manager
    {
        public Settings Current { get; set; } = Settings.Default;
        private XmlSerializer XmlSerializer { get; set; } = new XmlSerializer(typeof(Settings));

        public void Load()
        {
            if (File.Exists(Info.SettingsFullFileName()))
            {
                using (FileStream fileStream = new FileStream(Info.SettingsFullFileName(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    Current = (Settings)XmlSerializer.Deserialize(fileStream);
                }
                Manage.Logger.Add($"The application settings have been downloaded", LogType.Application, LogLevel.Debug);
            }
            else
            {
                Manage.Logger.Add($"Create default application settings", LogType.Application, LogLevel.Debug);
                Save();
            }
            Manage.EventManager.ExecuteEvent<IEventHandlerSettingsLoaded>(new SettingsLoadedEvent(Current));
        }
        public void Save()
        {
            Manage.Logger.Add($"Trying to save application settings", LogType.Application, LogLevel.Debug);
            if (File.Exists(Info.SettingsFullFileName()))
            {
                File.Delete(Info.SettingsFullFileName());
            }
            using (FileStream fileStream = new FileStream(Info.SettingsFullFileName(), FileMode.Create))
            {
                XmlSerializer.Serialize(fileStream, Current);
            }
            Manage.Logger.Add($"The application settings were saved", LogType.Application, LogLevel.Info);
        }
    }
}