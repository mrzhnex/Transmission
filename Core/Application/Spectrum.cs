using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LiveCharts.Geared;

namespace Core.Application
{
    public class Spectrum : INotifyPropertyChanged
    {
        private int KeepValues { get; set; } = 20000;

        public Spectrum()
        {
            Task.Run(Update);
        }

        public void SetKeepValues(int KeepValues)
        {
            this.KeepValues = KeepValues;
        }

        public GearedValues<int> Values { get; set; } = new GearedValues<int>().WithQuality(Quality.Highest);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<int> PreValues { get; set; } = new List<int>();

        public void Update()
        {
            while (true)
            {
                if (PreValues.Count == 0)
                    continue;
                int value = PreValues[0];
                PreValues.RemoveAt(0);
                var first = Values.DefaultIfEmpty(0).FirstOrDefault();
                if (Values.Count > KeepValues - 1) Values.Remove(first);
                if (Values.Count < KeepValues) Values.Add(value);
            }
        }

        public void ProcessData(List<byte> list)
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
                Amp.Add(volume);
                volume = 0;
            }
            PreValues.AddRange(Amp);
        }
    }
}