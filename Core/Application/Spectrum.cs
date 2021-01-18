using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LiveCharts.Geared;

namespace Core.Application
{
    public class Spectrum : INotifyPropertyChanged
    {
        private int KeepValues { get; set; } = 4500;

        public Spectrum()
        {
            Task.Run(Update);
        }

        public void SetKeepValues(int KeepValues)
        {
            this.KeepValues = KeepValues;
        }

        public GearedValues<int> Values { get; set; } = new GearedValues<int>().WithQuality(Quality.Low);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<int> PreValues { get; set; } = new List<int>();

        public void Update()
        {
            int length = 10;
            while (true)
            {
                if (PreValues.Count < length)
                    continue;
                Thread.Sleep(1);

                if (Values.Count > KeepValues - length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        Values.RemoveAt(0);
                    }
                }
                Values.AddRange(PreValues.GetRange(0, length));
                PreValues.RemoveRange(0, length);
            }
        }

        public void ProcessData(List<byte> list, bool silent = false)
        {
            byte[] buffer;
            List<int> Amp = new List<int>();
            int volume = 0;
            bool end = false;

            while (!end)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (list.Count >= 4)
                    {
                        buffer = list.GetRange(0, 4).ToArray();
                        list.RemoveRange(0, 4);
                        if (volume < BitConverter.ToInt16(buffer, 0))
                            volume = BitConverter.ToInt16(buffer, 0);
                    }
                    else
                    {
                        end = true;
                        break;
                    }
                }
                if (silent)
                    volume /= 100000;
                else
                    volume /= 100;
                Amp.Add(volume);
                volume = 0;
            }
            PreValues.AddRange(Amp);
        }
    }
}