using Core.Main;
using System;
using System.IO;

namespace Core.Localization
{
    public static class Info
    {
        private static string FolderName { get; set; } = "Languages";
        public static string FileFormat { get; set; } = "xml";
        public static int CultureMaxLength { get; set; } = 2;
        public static string DefaultLanguageCulture { get; set; } = "en";

        public static string GetFullFileName(string culture)
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal)), Manage.DefaultInformation.ApplicationName, FolderName, $"{culture}.{FileFormat}");
        }
        public static string GetFullFolderName()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal)), Manage.DefaultInformation.ApplicationName, FolderName);
        }
    }
}