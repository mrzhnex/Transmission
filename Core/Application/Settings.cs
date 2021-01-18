using System;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class Settings
    {
        public ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public ServerSettings ServerSettings { get; set; } = new ServerSettings();

        [XmlIgnore]
        public static Settings Default { get; set; } = new Settings();
        public Settings() { }
    }
}