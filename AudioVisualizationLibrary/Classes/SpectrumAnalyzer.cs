using AudioVisualizationLibrary.Controls;
using AudioVisualizationLibrary.Enums;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;
using static AudioVisualizationLibrary.Classes.MathF;

namespace AudioVisualizationLibrary.Classes
{
    public class SpectrumAnalyzer : IDisposable
    {
        #region Events 
        public delegate void SpectrumDataChangedEventHandler(double[] data);
        public event SpectrumDataChangedEventHandler SpectrumDataChanged;

        public delegate void ErrorEventHandler(string errorMessage);
        public event ErrorEventHandler OnError;

        public delegate void DeviceTypeChangedEventHandler(bool notFound);
        public event DeviceTypeChangedEventHandler OnDeviceListChanged;

        public delegate void InitializedEventHandler(object sender);
        public event InitializedEventHandler OnInitialized;
        #endregion

        private bool EnableValue;               //enabled status
        private DispatcherTimer Timer;         //timer that refreshes the display
        private readonly float[] FFTList;               //buffer for fft data
        private readonly ProgressInfo _l, _r;         //progressbars for left and right channel intensity
        private int _hanctr;                //last output level counter
        private double[] SpectrumData;   //spectrum data buffer
        public double[] SegmentData;
        private readonly AudioVisualizerControl Visualizer;         //spectrum dispay control
        private readonly DeviceListInfo _devicelist;       //device list
        private int devindex;               //used device index
        public bool Initialized;

        public double MaxValue;

        public int MinSignal = 64;
        public int MaxSignal = 192;

        public int SignalRepeatMode = 1;
        public int SignalChangeTime = 10;

        private int SampleCountValue = 180;
        public int SampleCount
        {
            get => this.SampleCountValue;
            set
            {
                this.SampleCountValue = value;
                this.Update();
            }
        }

        public SignalType SignalType = SignalType.Normal;
        public SignalOutputMode SignalOutputMode = SignalOutputMode.Normal;

        public MultiSineModes MultiSineMode = MultiSineModes.Absolute;

        private Point Center;

        public bool InvertSignal = false;

        private int BarCount;
        private bool Invert = false;
        private DateTime LastChange;

        public static readonly int MaxFFT = 1024;




        private readonly SignalGenerator SignalGenerator;

        private readonly Random[] Random;
   
        //ctor
        public SpectrumAnalyzer(int sampleCount, ProgressInfo left, ProgressInfo right, AudioVisualizerControl visualizer, DeviceListInfo devicelist)
        {
            this.Visualizer = visualizer;

            this.Random = new Random[3];

            for (int i = 0; i < this.Random.Length; i++)
            {
                this.Random[i] = new Random();
            }

            this.SignalGenerator = new SignalGenerator();

            this.SampleCountValue = sampleCount;
            this.FFTList = new float[MaxFFT];
            this._hanctr = 0;

            this.Timer = new DispatcherTimer();
            this.Timer.Tick += new EventHandler(this.Tick);
            this.Timer.Interval = TimeSpan.FromMilliseconds(25); //40hz refresh rate
            this.Timer.IsEnabled = false;

            this._l = left;
            this._r = right;

            this._l.Minimum = this._r.Minimum = 0;
            this._l.Maximum = this._r.Maximum = ushort.MaxValue;

            
            this.SpectrumData = new double[this.SampleCountValue];

            this._devicelist = devicelist;
            this.Initialized = false;


            this.LastChange = DateTime.Now;

            this.Update();
            this.Init();
        }

        #region Dispose
        private bool Disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    // Dispose any managed objects
                    this.Enable = false;

                    if (this.Timer != null)
                    {
                        this.Timer.IsEnabled = false;
                        this.Timer.Stop();
                        this.Timer.Tick -= new EventHandler(this.Tick);
                        this.Timer = null;
                    }

                    Array.Clear(this.SpectrumData, 0, this.SpectrumData.Length);
                    Array.Clear(this.SegmentData, 0, this.SegmentData.Length);

                    this.SpectrumData = null;
                    this.SegmentData = null;

                    if (this.Random != null)
                    {
                        for (int i = 0; i < this.Random.Length; i++)
                            this.Random[i] = null;
                    }
                }

                // Now disposed of any unmanaged objects
                this.Disposed = true;
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~SpectrumAnalyzer()
        {
            this.Dispose(false);
        }
        #endregion

        // Serial port for arduino output
        public SerialPort Serial;

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get => this.EnableValue;
            set
            {
                this.EnableValue = value;

                if (!this.Visualizer.IsDesignMode)
                {
                    if (value)
                    {
                        if (!this.Initialized && this._devicelist.Items.Count > 0)
                        {
                            var array = Common.FormatDeviceName(this._devicelist.Items[this._devicelist.SelectedIndex]).Split(' ');

                            devindex = Convert.ToInt32(array[0]);
                            this.Initialized = true;
                            this._devicelist.IsEnabled = false;
                            this.OnInitialized?.Invoke(this);
                        }
                    }

                    if (this.Timer != null)
                    {
                        this.Timer.IsEnabled = value;
                    }
                }
            }
        }

        // initialization
        private void Init()
        {
            if (this.Visualizer.IsDesignMode) return;

            this._devicelist.SelectedIndex = -1;
            this._devicelist.Items.Clear();

            bool result = false;
            this._devicelist.Items.Add(string.Format("{0} - {1}", 0, Common.FormatDeviceName("BASS_WASAPI_TYPE_UNKNOWN")));

            if (this._devicelist.Items.Count > 0)
            {
                this._devicelist.SelectedIndex = 0;

                this.OnDeviceListChanged?.Invoke(false);

                if (!result)
                {
                    this.OnError?.Invoke("Init Error");
                }
            }
            else
                this.OnDeviceListChanged?.Invoke(true);
        }

        //timer 
        private void Tick(object sender, EventArgs e)
        {
            int ret = 0; //get channel fft data

            if (ret < -1) return;

            bool reverse = false;

            int center = this.SampleCountValue / 2;

            int index, x, y;
            int b0 = 0, b1 = 0;
            double peak = 0;

            //reset
            Array.Clear(this.SegmentData, 0, this.SegmentData.Length);
            Array.Clear(this.SpectrumData, 0, this.SpectrumData.Length);

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            if (this.SignalType == SignalType.Normal)
            {
                for (x = 0; x < this.SampleCountValue; x++)
                {
                    b1 = (int)Math.Pow(2, x * 10.0 / (this.SampleCountValue - 1));
                    peak = 0;

                    if (b1 > 1023) b1 = 1023;
                    if (b1 <= b0) b1 = b0 + 1;

                    for (; b0 < b1; b0++)
                    {
                        if (peak < this.FFTList[1 + b0]) peak = this.FFTList[1 + b0];
                    }

                    y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);

                    if (y > 255) y = 255;
                    if (y < 0) y = 0;

                    y = y > 0 && y <= this.MinSignal ? this.MinSignal : y;
                    y = y >= this.MaxSignal ? this.MaxSignal : y;

                    if (x < center)
                    {
                        this.SpectrumData[x] = y;
                    }
                    else
                    {
                        if (y == 0)
                        {
                            //reverse or repeat
                            index = reverse ? center - (x - center) : x - center;

                            this.SpectrumData[x] = this.SpectrumData[index];
                        }
                        else
                            this.SpectrumData[x] = y;
                    }

                    //Console.Write("{0, 3} ", y);
                }
            }
            else
            {
                b0 = 0;

                int maxDataSector = -1;
                int minDataSector = -1;
                double maxData = byte.MinValue;
                double minData = byte.MaxValue;
                double data = byte.MinValue;
                double total = 0;
                int count = this.SegmentData.Length - 1;

                int randomSector = this.Random[0].Next(0, this.SegmentData.Length);

                for (x = 0; x < this.SegmentData.Length; x++)
                {
                    b1 = (int)Math.Pow(2, x * 10.0 / count);
                    peak = 0;

                    if (b1 > 1023) b1 = 1023;
                    if (b1 <= b0) b1 = b0 + 1;

                    for (; b0 < b1; b0++)
                    {
                        if (peak < this.FFTList[1 + b0]) peak = this.FFTList[1 + b0];
                    }

                    y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);

                    if (y > 255) y = 255;
                    if (y < 0) y = 0;

                    y = y > 0 && y <= this.MinSignal ? this.MinSignal : y;
                    y = y >= this.MaxSignal ? this.MaxSignal : y;

                    if (this.SignalType == SignalType.MultiSine || this.SignalType == SignalType.MultiTriangle)
                    {
                        if (this.MultiSineMode == MultiSineModes.Random || this.MultiSineMode == MultiSineModes.MultiSineRandom)
                            this.SegmentData[x] = y == 0 ? y : this.Random[1].Next(y, this.MaxSignal);
                        else
                            this.SegmentData[x] = y;

                        if (x == this.SegmentData.Length - 1)
                        {
                            this.SegmentData[x] = this.SegmentData[0];
                        }
                    }
                    else
                    {
                        //iki bölümlü yapı için
                        //this.SectorData[x] = x % 2 == 0 ? (byte)y : this.SectorData[x - 1];
                        switch (this.SignalOutputMode)
                        {
                            case SignalOutputMode.Absolute:
                                this.SegmentData[x] = x == 0 ? y : this.SegmentData[x - 1];
                                break;
                            case SignalOutputMode.AbsoluteHalf:

                                if (x == 0)
                                {
                                    data = y;
                                }

                                this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? data : data / 2;

                                break;
                            case SignalOutputMode.InvertedAbsoluteHalf:

                                if (x == 0)
                                {
                                    data = y;
                                }

                                if (this.Invert)
                                    this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? data / 2 : data;
                                else
                                    this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? data : data / 2;

                                break;
                            case SignalOutputMode.Average:
                                this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? y : (this.SegmentData[x - 1] + y) / 2;
                                break;
                            case SignalOutputMode.InvertedAverage:

                                if (this.Invert)
                                {
                                    if (x == 0)
                                        this.SegmentData[x] = y / 2;
                                    else
                                        this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? y : (this.SegmentData[x - 1] + y) / 2;
                                }
                                else
                                {
                                    if (x == 0)
                                        this.SegmentData[x] = y;
                                    else
                                        this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? (this.SegmentData[x - 1] + y) / 2 : y;
                                }

                                break;
                            case SignalOutputMode.Modular:
                                this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? y : byte.MinValue;
                                break;
                            case SignalOutputMode.ModularHalf:
                                this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? y : (y / 2);
                                break;
                            case SignalOutputMode.InvertedModular:
                                this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? this.Invert ? y : byte.MinValue : this.Invert ? byte.MinValue : y;
                                break;
                            case SignalOutputMode.InvertedModularHalf:

                                if (this.Invert)
                                    this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? (y / 2) : y;
                                else
                                    this.SegmentData[x] = x % this.SignalRepeatMode == 0 ? y : (y / 2);

                                break;
                            case SignalOutputMode.Normal:
                                this.SegmentData[x] = y;
                                break;
                            case SignalOutputMode.Max:

                                if (maxData < y)
                                {
                                    maxData = y;
                                    maxDataSector = x;
                                }

                                this.SegmentData[x] = byte.MinValue;

                                break;
                            case SignalOutputMode.Min:

                                if (minData > y)
                                {
                                    minData = y;
                                    minDataSector = x;
                                }

                                this.SegmentData[x] = byte.MinValue;

                                break;
                            case SignalOutputMode.MaxAndMin:


                                if (maxData < y)
                                {
                                    maxData = y;
                                    maxDataSector = x;
                                }

                                if (minData > y)
                                {
                                    minData = y;
                                    minDataSector = x;
                                }

                                this.SegmentData[x] = byte.MinValue;

                                break;

                            case SignalOutputMode.Random:

                                this.SegmentData[x] = x == randomSector ? y : byte.MinValue;

                                break;
                        }
                    }

                    total += this.SegmentData[x];
                }

                if (minDataSector != -1)
                {
                    this.SegmentData[minDataSector] = minData;
                    total += minData;
                }
                if (maxDataSector != -1)
                {
                    this.SegmentData[maxDataSector] = maxData;
                    total += maxData;
                }

                index = 0;

                switch (this.SignalType)
                {
                    case SignalType.MultiSine:
                    case SignalType.MultiTriangle:
                        {
                            int start = 0, end = 0;

                            double r, frequency;

                            double radius = this.MaxValue / 2.0;
                            
                            Point c = new Point(this.Center.X, this.Center.Y - radius);

                            switch (this.MultiSineMode)
                            {
                                case MultiSineModes.Absolute:
                                    {
                                        for (x = 0; x < this.SegmentData.Length; x++)
                                        {
                                            this.SignalGenerator.Frequency = 1.0 / (this.BarCount / .5);
                                            this.SignalGenerator.Amplitude = this.SegmentData[x] / byte.MaxValue;

                                            for (y = 0; y < this.BarCount; y++)
                                            {
                                                r = this.SignalGenerator.GetValue(this.GetTime(y, this.BarCount)) * radius;

                                                //üst bölüm
                                                if (x % 2 == 0)
                                                    this.SpectrumData[index++] = MathF.GetDistance(0, this.Center.Y, 0, c.Y - r);
                                                //alt bölüm
                                                else
                                                    this.SpectrumData[index++] = MathF.GetDistance(0, this.Center.Y, 0, c.Y + r);
                                            }
                                        }

                                        break;
                                    }
                                case MultiSineModes.Percentage:
                                case MultiSineModes.Random:
                                    {
                                        for (x = 0; x < this.SegmentData.Length; x++)
                                        {
                                            end += (int)Math.Round(this.SegmentData[x] * Convert.ToDouble(this.SpectrumData.Length) / total);

                                            //son sektöre gelindiğinde döngünün atladığı barlar için düzeltme yap.
                                            if (x == this.SegmentData.Length - 1)
                                                end += this.SpectrumData.Length - end;

                                            count = end - start;

                                            this.SignalGenerator.Frequency = 1.0 / (count / .5);
                                            this.SignalGenerator.Amplitude = this.SegmentData[x] / byte.MaxValue;

                                            for (y = 0; y < count; y++)
                                            {
                                                if (index < this.SpectrumData.Length)
                                                {
                                                    r = this.SignalGenerator.GetValue(this.GetTime(y, count)) * radius;

                                                    //üst bölüm
                                                    if (x % 2 == 0)
                                                        this.SpectrumData[index++] = MathF.GetDistance(0, this.Center.Y, 0, c.Y - r);
                                                    //alt bölüm
                                                    else
                                                        this.SpectrumData[index++] = MathF.GetDistance(0, this.Center.Y, 0, c.Y + r);
                                                }
                                            }

                                            start = end;
                                        }

                                        break;
                                    }
                                case MultiSineModes.MultiSineAbsolute:
                                    {
                                        frequency = 1.0 / this.BarCount;

                                        for (x = 0; x < this.SegmentData.Length; x++)
                                        {
                                            for (y = 0; y < this.BarCount; y++)
                                            {
                                                this.SpectrumData[index++] = Math.Abs(MathF.MultiSine(0, this.GetTime(y, this.BarCount, x, 2) * frequency)) * this.SegmentData[x];
                                            }
                                        }

                                        break;
                                    }
                                case MultiSineModes.MultiSinePercentage:
                                case MultiSineModes.MultiSineRandom:
                                    {
                                        for (x = 0; x < this.SegmentData.Length; x++)
                                        {
                                            end += (this.SegmentData[x] * Convert.ToDouble(this.SpectrumData.Length - 1) / total).Round(false);

                                            //son sektöre gelindiğinde döngünün atladığı barlar için düzeltme yap.
                                            if (x == this.SegmentData.Length - 1)
                                                end += this.SpectrumData.Length - end;

                                            count = end - start;
                                            frequency = 1.0 / count;

                                            for (y = 0; y < count; y++)
                                            {
                                                this.SpectrumData[index++] = Math.Abs(MathF.MultiSine(0, this.GetTime(y, count, x, 2) * frequency)) * this.SegmentData[x];
                                            }

                                            start = end;
                                        }

                                        break;
                                    }
                            }

                            break;
                        }
                    default:
                        {
                            for (x = 0; x < this.SegmentData.Length; x++)
                            {
                                this.SignalGenerator.Frequency = 1.0 / (this.BarCount / .5);
                                this.SignalGenerator.Amplitude = this.SegmentData[x] / byte.MaxValue;

                                for (y = 0; y < this.BarCount; y++)
                                {
                                    this.SpectrumData[index++] = Math.Abs(this.SignalGenerator.GetValue(this.GetTime(y, this.BarCount))) * byte.MaxValue;
                                }
                            }

                            break;
                        }
                }

                TimeSpan elapsed = DateTime.Now - this.LastChange;

                if (elapsed.Seconds > this.SignalChangeTime)
                {
                    this.Invert = !this.Invert;
                    this.LastChange = DateTime.Now;
                }
            }

            this.SpectrumDataChanged?.Invoke(this.SpectrumData);

            /*if (this.Serial != null)
            {
                this.Serial.Write(this.SpectrumData, 0, this.SpectrumData.Length);
            }*/




            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            if (this._hanctr > 3)
            {
                this._hanctr = 0;
                this._l.Value = this._r.Value = 0;
                this.Initialized = false;
                this.Enable = true;
            }
        }

        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user) => length;

        //cleanup

        #region Signal Generator
        private double GetTime(double num, double count)
        {
            return this.InvertSignal ? count - num : num;
        }
        private double GetTime(double num, double count, int sectorIndex, int modVal)
        {
            return sectorIndex % modVal == 0 ? num : count - num;
        }
        private void Update()
        {
            if (!this.Visualizer.IsDesignMode)
            {
                this.SpectrumData = new double[this.SampleCountValue];
                this.Center = this.Visualizer.Center;

                this.SegmentData = new double[this.Visualizer.SectorCountValue];
                this.BarCount = this.Visualizer.SectorBarCountValue;

                switch (this.SignalType)
                {
                    case SignalType.MultiSine:

                        switch (this.MultiSineMode)
                        {
                            default:

                                this.MaxValue = byte.MaxValue;

                                break;
                        }

                        break;
                    default:

                        this.MaxValue = byte.MaxValue;

                        break;
                }

                this.SignalGenerator.Frequency = 1.0 / (this.BarCount / .5);
                this.SignalGenerator.Phase = 0.0;
                this.SignalGenerator.Amplitude = 1.0;
                this.SignalGenerator.Offset = 0.0;
                this.SignalGenerator.Invert = this.InvertSignal;
                this.SignalGenerator.SignalType = this.SignalType;
            }
        }
        #endregion
    }
}
