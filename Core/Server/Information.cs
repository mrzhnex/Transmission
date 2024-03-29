﻿using Mono.Nat;
using NAudio.Wave;

namespace Core.Server
{
    public class Information
    {
        public string ClientInfoMessage { get; set; } = nameof(ClientInfoMessage);
        public string ApplicationName { get; set; } = "Transmission";
        public string SessionName { get; set; } = "сессия";
        public string DefaultFileName { get; set; } = "не выбран";
        public string DefaultFontFamily { get; set; } = "Arial";
        public int DataLength { get; set; } = 8820;
        public Protocol Protocol { get; set; } = Protocol.Udp;
        public string DisconnectMessage { get; set; } = nameof(DisconnectMessage);
        public int VerificationUpdateTime { get; set; } = 1500;
        public WaveFormat WaveFormat { get; set; } = new WaveFormat(44100, 16, 1);
        public string PortDescription { get; set; } = nameof(Server);
        public char SplitSymbol { get; set; } = ';';
        public string VerificationMessage { get; set; } = nameof(VerificationMessage);
        public int ServerDelay { get; set; } = 500;
    }
}