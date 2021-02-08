using Core.Events;
using Core.Handlers;
using Core.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Media;

namespace Core.Application
{
    public class Manager
    {
        public Settings Current { get; set; } = Settings.Default;
        private XmlSerializer XmlSerializer { get; set; } = new XmlSerializer(typeof(Settings));
        public static Dictionary<ThemeType, Theme> Themes { get; set; } = new Dictionary<ThemeType, Theme>()
        {
            { ThemeType.Default, new Theme()
            {
                FirstColor = new SolidColorBrush(System.Drawing.Color.Thistle.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleTurquoise.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.PaleGreen.ToMediaColor()),
                FourthColor = new SolidColorBrush(System.Drawing.Color.MediumPurple.ToMediaColor())
            } },
            { ThemeType.DefaultTwo,  new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.Thistle.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleTurquoise.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.NavajoWhite.ToMediaColor()),
                FourthColor =new SolidColorBrush(System.Drawing.Color.MediumPurple.ToMediaColor()) } },
            { ThemeType.Windows,  new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.CornflowerBlue.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleGoldenrod.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.LightSkyBlue.ToMediaColor()),
                FourthColor =new SolidColorBrush(System.Drawing.Color.DeepSkyBlue.ToMediaColor()) } }
        };


        public void Load()
        {
            if (File.Exists(Info.SettingsFullFileName()))
            {
                try
                {
                    using (FileStream fileStream = new FileStream(Info.SettingsFullFileName(), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        Current = (Settings)XmlSerializer.Deserialize(fileStream);
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

        private void CreateNewAndSave()
        {
            Manage.Logger.Add($"Create default application settings", LogType.Application, LogLevel.Debug);
            Save();
        }
    }
}