using Core.Main;
using System;
using System.IO;

namespace Core.Application
{
    public static class Info
    {
        private static string SettingsFileName { get; set; } = "Settings.xml";
        public static string SettingsFullFileName()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Manage.DefaultInformation.ApplicationName, SettingsFileName));
        }
    }
}