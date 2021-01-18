using System;
using System.Xml.Serialization;

namespace Core.Application
{
    [Serializable]
    public class ClientSettings
    {
        public string ClientName { get; set; } = "Client Name";
        public int RecordSaveTime { get; set; } = 10;
        public bool OutputMuteStatus { get; set; } = false;
        public bool InputMuteStatus { get; set; } = false;
        public float InputVolumeValue { get; set; } = 1.0f;
        public float OutputVolumeValue { get; set; } = 1.0f;

        [XmlIgnore]
        public static ClientSettings Default { get; set; } = new ClientSettings();
        public  ClientSettings() { }
    }
}