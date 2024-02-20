using Core.Main;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Core.Application
{
    public class Record
    {
        public bool IsRecording { get; private set; }
        public bool IsStartRecording { get; private set; }
        public List<byte> RecordingAudio { get; private set; } = new List<byte>();
        private DateTime StartTime { get; set; } = DateTime.Now;
        private DateTime EndTime { get; set; }
        private Server.Client Client { get; set; }
        private string Name { get; set; } = string.Empty;
        private bool Active { get; set; } = true;
        private bool IsClient { get; set; } = false;

        public Record(string Name, Server.Client Client, bool IsClient, bool IsRecording)
        {
            this.Client = Client;
            this.IsClient = IsClient;
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
            IsStartRecording = false;
            IsRecording = false;
            EndTime = DateTime.Now;
            if (RecordingAudio.Count > 0)
            {
                Manage.Logger.Add("Trying to save a record", LogType.Application, LogLevel.Debug);
                try
                {
                    string fileFullName = IsClient ? Manage.ApplicationManager.ClientSettings.RecordSaveFolder : Manage.ApplicationManager.ServerSettings.RecordSaveFolder;
                    if (fileFullName == string.Empty)
                        fileFullName = Manage.Logger.LogsFolder;
                    fileFullName = Path.Combine(fileFullName, $"{GetStartTime()} - {GetEndTime()}_{Name}.wav");
                    WaveFileWriter waveFileWriter = new WaveFileWriter(fileFullName, Manage.DefaultInformation.WaveFormat);
                    waveFileWriter.WriteData(RecordingAudio.ToArray(), 0, RecordingAudio.ToArray().Length);
                    waveFileWriter.Close();
                    RecordingAudio = new List<byte>();
                    Manage.Logger.Add($"The record was saved at {fileFullName}", LogType.Application, LogLevel.Info);
                }
                catch (Exception ex)
                {
                    Manage.Logger.Add($"Catch an exception {ex.Message} while saving record data", LogType.Application, LogLevel.Error);
                }
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
            IsStartRecording = true;
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