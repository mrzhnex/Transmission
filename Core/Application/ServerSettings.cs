using System;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ServerSettings
    {
        public int RecordSaveTime { get; set; } = 10;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 5555;
        public string SaveFolder { get; set; } = string.Empty;
        public string RecordSaveFolder { get; set; } = string.Empty;
        public string PlayAudioFile { get; set; } = string.Empty;
        [XmlIgnore]
        public static ServerSettings Default { get; set; } = new ServerSettings();
        public ServerSettings() { }
    }
}