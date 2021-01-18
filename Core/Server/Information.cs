using Mono.Nat;
using NAudio.Wave;

namespace Core.Server
{
    public class Information
    {
        public string ApplicationName { get; set; } = "Transmission";
        public string SessionName { get; set; } = "Session";
        public int DataLength { get; set; } = 8820;
        public Protocol Protocol { get; set; } = Protocol.Udp;
        public string DisconnectMessage { get; set; } = nameof(DisconnectMessage);
        public int VerificationUpdateTime { get; set; } = 500;
        public WaveFormat WaveFormat { get; set; } = new WaveFormat(44100, 16, 1);
        public string PortDescription { get; set; } = nameof(Server);
        public char SplitSymbol { get; set; } = ';';
        public string VerificationMessage { get; set; } = nameof(VerificationMessage);
        public int ServerDelay { get; set; } = 100;
    }
}