using Core.Main;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Application
{
    public class Record
    {
        public bool IsRecording { get; private set; }
        public List<byte> RecordingAudio { get; private set; } = new List<byte>();
        private DateTime StartTime { get; set; } = DateTime.Now;
        private DateTime EndTime { get; set; }
        private Server.Client Client { get; set; }
        private string Name { get; set; } = string.Empty;
        private bool Active { get; set; } = true;

        public Record(string Name, Server.Client Client, bool IsRecording = false)
        {
            this.Client = Client;
            this.Name = Name;
            this.IsRecording = IsRecording;
        }

        private void Recording()
        {
            while (Active)
            {
                if (IsRecording)
                {
                    byte[] recordData = Client.GetAudio();
                    if (Manage.GetUlongFromBuffer(recordData) > 0)
                    {
                        RecordingAudio.AddRange(recordData);
                    }
                }
                Thread.Sleep(Manage.DefaultInformation.ServerDelay);
            }
        }

        public void Pause()
        {
            IsRecording = false; ;
        }

        public void Play()
        {
            IsRecording = true;
        }

        public void Save()
        {
            Active = false;
            IsRecording = false;
            EndTime = DateTime.Now;
            if (RecordingAudio.Count > 0)
            {
                Manage.Logger.Add("Trying to save a record", LogType.Application, LogLevel.Debug);
                WaveFileWriter waveFileWriter = new WaveFileWriter($"{GetStartTime()} - {GetEndTime()}_{Name}.wav", Manage.DefaultInformation.WaveFormat);
                waveFileWriter.WriteData(RecordingAudio.ToArray(), 0, RecordingAudio.ToArray().Length);
                waveFileWriter.Close();
                Manage.Logger.Add("The record was saved", LogType.Application, LogLevel.Info);
            }
            else
            {
                Manage.Logger.Add("Saving the record was canceled", LogType.Application, LogLevel.Debug);
            }
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            RecordingAudio = new List<byte>();
            IsRecording = true;
            Active = true;
            new Thread(delegate ()
            {
                Recording();
            }).Start();
        }

        private string GetStartTime()
        {
            return $"{StartTime.Date.Year}.{StartTime.Date.Month}.{StartTime.Date.Day}_{StartTime.Hour}.{StartTime.Minute}.{StartTime.Second}";
        }
        private string GetEndTime()
        {
            return $"{EndTime.Date.Year}.{EndTime.Date.Month}.{EndTime.Date.Day}_{EndTime.Hour}.{EndTime.Minute}.{EndTime.Second}";
        }
    }
}