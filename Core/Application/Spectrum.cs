using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Main;
using LiveCharts.Geared;

namespace Core.Application
{
    public class Spectrum
    {
        private int KeepValues { get; set; } = 10000;
        public GearedValues<double> Values { get; set; } = new GearedValues<double>().WithQuality(Quality.Low);
        private List<double> PreValues { get; set; } = new List<double>();

        public Spectrum()
        {
            new Thread(Update).Start();
        }

        public void SetKeepValues(int KeepValues)
        {
            this.KeepValues = KeepValues;
        }

        private void Update()
        {
            int length = 70;
            
            while (Manage.Logger.ActiveLog)
            {
                lock (PreValues)
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
        }

        public void ProcessData(byte[] input, bool silent = false)
        {
            Task.Run(new Action(() => CoreProcessData(input, silent)));
        }

        public void ClearPreValues()
        {
            lock (PreValues)
                PreValues.Clear();
        }

        private unsafe void CoreProcessData(byte[] input, bool silent = false)
        {
            var bufferA = new double[input.Length / 4];
            fixed (byte* pSource = input)
            fixed (double* pBufferA = bufferA)
            {
                var pLen = pSource + input.Length;
                double* pA = pBufferA;
                for (var pS = pSource; pS < pLen; pS += 4, pA++)
                {
                    if (silent)
                        *pA = *(short*)pS / 100000;
                    else
                        *pA = *(short*)pS / 200;
                    if (*pA < 0)
                        *pA = 0;
                    PreValues.Add(*pA);
                }
            }
        }
    }
}