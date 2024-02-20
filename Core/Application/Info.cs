using Core.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Core.Application
{
    public static class Info
    {
        public static List<FontFamily> FontFamilies { get; set; } = Fonts.SystemFontFamilies.Where(x => !x.Source.ToLower().Contains("microsoft") && !x.Source.ToLower().Contains("segoe") && !x.Source.ToLower().Contains("sitka") && !x.Source.ToLower().Contains("ms ") && !x.Source.ToLower().Contains("wingdings") && !x.Source.ToLower().Contains("webdings") && x.Baseline.ToString().Length > 4).ToList();

        private static string SettingsFileName { get; set; } = "Settings.xml";

        public static string ClientSettingsFullFileName()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Manage.DefaultInformation.ApplicationName, "Client" + SettingsFileName));
        }
        public static string ServerSettingsFullFileName()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Manage.DefaultInformation.ApplicationName, "Server" + SettingsFileName));
        }
    }
}