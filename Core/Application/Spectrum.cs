using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Main;

namespace Core.Application
{
    public class Spectrum
    {
        private int KeepValues { get; set; } = 1000;
        public List<double> Values { get; set; } = new List<double>();
        private List<double> PreValues { get; set; } = new List<double>();

        public Spectrum()
        {
            new Thread(Update).Start();
            new Thread(AddVoidValues).Start();
        }

        private void AddVoidValues()
        {
            while (Manage.Logger.ActiveLog)
            {
                Thread.Sleep(500);
                ProcessData(new byte[10000], true);
            }
        }


        private void Update()
        {
            int length = 500;
            
            while (Manage.Logger.ActiveLog)
            {
                Thread.Sleep(1);
                lock (PreValues)
                {
                    if (PreValues.Count < length)
                        continue;
                    lock (Values)
                    {
                        if (Values.Count > KeepValues)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                Values.RemoveAt(0);
                            }
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
            try
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
                            *pA = *(short*)pS / 80;
                        if (*pA < 0)
                            *pA = 0;
                        PreValues.Add(*pA);
                    }
                }
            }
            catch (Exception ex)
            {
                Manage.Logger.Add($"Catch an exception {ex.Message} during {nameof(CoreProcessData)}", LogType.Application, LogLevel.Error);
            }
        }
    }
}