using Core.Application;
using AudioVisualizationLibrary.Classes.ColorLibrary;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using static AudioVisualizationLibrary.Classes.MathF;
using AudioVisualizationLibrary.Classes;
using Client.Controls;
using Core.Main;

namespace Client
{
    public partial class SpectrumControl : UserControl
    {
        private XElement OptionsRoot { get; set; } = new XElement("Options",

                    new XElement("Device",
                    new XAttribute("Index", "0"),
                    new XAttribute("Type", "BASS_WASAPI_TYPE_UNKNOWN")),

                    new XElement("WaveFactor", "0,9"),
                    new XElement("SelectedTab", "Options"),
                    new XElement("Space", "0"),
                    new XElement("BarSpace", "5"),
                    new XElement("VerticalBarSpace", "0"),
                    new XElement("VerticalBarCount", "50"),

                    new XElement("Signal",
                    new XAttribute("Type", "Normal"),
                    new XAttribute("MultiSineMode", "Absolute"),
                    new XAttribute("OutputMode", "Normal"),
                    new XAttribute("ChangeTime", "5"),
                    new XAttribute("RepeatMode", "1"),
                    new XAttribute("Max", "255"),
                    new XAttribute("Min", "0"),
                    new XAttribute("Invert", "false"),
                    new XAttribute("ShiftSignals", "None"),
                    new XAttribute("SectorCount", "12"),
                    new XAttribute("SectorBarCount", "10")),

                    new XElement("MultiColor",
                    new XAttribute("Mode", "None"),
                    new XAttribute("StartColor", "DodgerBlue"),
                    new XAttribute("EndColor", "Navy"),
                    new XAttribute("HueStartFactor", "0"),
                    new XAttribute("HueEndFactor", "360"),
                    new XAttribute("ShiftDirection", "None"),
                    new XAttribute("ColorTone", "0,5"),
                    new XAttribute("InvertColorRange", "false"),
                    new XAttribute("ColorRepeatCount", "1")),

                    new XElement("SideCount", "3"),

                    new XElement("VisualizerType", "Bar"),
                    new XElement("RotateDirection", "None"),
                    new XElement("PeakDownSpeed", "5"),

                    new XElement("ShowProgress", "true"),

                    new XElement("VisualizerOpacity", "1,0"),
                    new XElement("BackgroundOpacity", "1,0"),

                    new XElement("Test",
                    new XAttribute("Mode", "None")),

                    new XElement("Language", CultureInfo.CurrentCulture.Name == "tr-TR" ? "TR" : "EN"),
                    new XElement("Topmost", "false"),
                    new XElement("PlayerAutoHide", "false"),
                    new XElement("HideMessages", "true"),

                    new XElement("Player",
                    new XAttribute("File", "")),

                    new XElement("Screenshot",
                    new XAttribute("Width", "1024"),
                    new XAttribute("Height", "768")),

                    new XElement("Background",
                    new XAttribute("Color", "Black"),
                    new XAttribute("Image", ""),
                    new XAttribute("Video", ""),
                    new XAttribute("Type", "Color"),
                    new XAttribute("TileMode", "None"),
                    new XAttribute("StretchMode", "None")),

                    new XElement("Bar",
                    new XAttribute("Color", "Blue"),
                    new XAttribute("StartLineCap", "Flat"),
                    new XAttribute("DashCap", "Flat"),
                    new XAttribute("DashStyle", "Solid"),
                    new XAttribute("EndLineCap", "Flat")),

                    new XElement("Peak",
                    new XAttribute("Color", "Blue"),
                    new XAttribute("StartLineCap", "Flat"),
                    new XAttribute("DashCap", "Flat"),
                    new XAttribute("DashStyle", "Solid"),
                    new XAttribute("EndLineCap", "Flat")),

                    new XElement("Progress",
                    new XAttribute("Color", "Crimson"),
                    new XAttribute("LineColor", "Black"),
                    new XAttribute("StartLineCap", "Flat"),
                    new XAttribute("DashCap", "Flat"),
                    new XAttribute("DashStyle", "Solid"),
                    new XAttribute("EndLineCap", "Flat"),
                    new XAttribute("LineStartLineCap", "Flat"),
                    new XAttribute("LineEndLineCap", "Flat"))
                );
        private DispatcherTimer Timer { get; set; }
        private double[] Data { get; set; } = new double[120];
        private Spectrum Spectrum { get; set; } = new Spectrum();
        public bool CanProcess { get; set; } = true;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            XElement[] elements = OptionsRoot.Elements().ToArray();
            XElement element;
            XAttribute[] attributes;
            XAttribute attribute;

            for (int x = 0; x < elements.Length; x++)
            {
                element = elements[x];

                if (!this.OptionsRoot.Elements().Any(f => f.Name == element.Name))
                {
                    this.OptionsRoot.Add(element);
                }

                XElement optElement = this.OptionsRoot.Elements().First(f => f.Name == element.Name);

                attributes = element.Attributes().ToArray();

                for (int y = 0; y < attributes.Length; y++)
                {
                    attribute = attributes[y];

                    if (!optElement.Attributes().Any(f => f.Name == attribute.Name))
                    {
                        optElement.SetAttributeValue(attribute.Name, attribute.Value);
                    }
                }
            }

            elements = this.OptionsRoot.Elements().ToArray();

            for (int x = 0; x < elements.Length; x++)
            {
                element = elements[x];
                attributes = element.Attributes().ToArray();

                for (int y = 0; y < attributes.Length; y++)
                {
                    attribute = attributes[y];

                    if (!OptionsRoot.Elements().First(f => f.Name == element.Name).Attributes().Any(f => f.Name == attribute.Name))
                    {
                        attribute.Remove();
                    }
                }

                if (!OptionsRoot.Elements().Any(f => f.Name.LocalName == element.Name.LocalName))
                {
                    element.Remove();
                }
            }

            this.Visualizer.BarBrush = ColorPicker.Colors.First(f => f.Name == this.OptionsRoot.Element("Bar").Attribute("Color").Value).Color.ToBrush();
            this.Visualizer.BarStartLineCap = this.OptionsRoot.Element("Bar").Attribute("StartLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.BarDashCap = this.OptionsRoot.Element("Bar").Attribute("DashCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.BarEndLineCap = this.OptionsRoot.Element("Bar").Attribute("EndLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.BarDashStyle = PenEditor.GetDashStyle(this.GetOption("Bar", "DashStyle")).Style;

            this.Visualizer.PeakBrush = ColorPicker.Colors.First(f => f.Name == this.OptionsRoot.Element("Peak").Attribute("Color").Value).Color.ToBrush();
            this.Visualizer.PeakStartLineCap = this.OptionsRoot.Element("Peak").Attribute("StartLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.PeakDashCap = this.OptionsRoot.Element("Peak").Attribute("DashCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.PeakEndLineCap = this.OptionsRoot.Element("Peak").Attribute("EndLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.PeakDashStyle = PenEditor.GetDashStyle(this.GetOption("Peak", "DashStyle")).Style;

            this.Visualizer.ProgressBrush = ColorPicker.Colors.First(f => f.Name == this.OptionsRoot.Element("Progress").Attribute("Color").Value).Color.ToBrush();
            this.Visualizer.ProgressLineBrush = ColorPicker.Colors.First(f => f.Name == this.OptionsRoot.Element("Progress").Attribute("LineColor").Value).Color.ToBrush();
            this.Visualizer.ProgressStartLineCap = this.Visualizer.ProgressLineStartLineCap = this.OptionsRoot.Element("Progress").Attribute("StartLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.ProgressDashCap = this.OptionsRoot.Element("Progress").Attribute("DashCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.ProgressEndLineCap = this.Visualizer.ProgressLineEndLineCap = this.OptionsRoot.Element("Progress").Attribute("EndLineCap").Value.ToEnum<PenLineCap>();
            this.Visualizer.ProgressDashStyle = PenEditor.GetDashStyle(this.GetOption("Progress", "DashStyle")).Style;


            SignalType[] signalTypes = { SignalType.Normal, SignalType.Sawtooth, SignalType.Sine, SignalType.MultiSine, SignalType.MultiTriangle, SignalType.Square, SignalType.Triangle };

            this.SetHandlers(false);
        }

        private void OnSpectrumDataChanged(double[] data)
        {
            if (CanProcess && Spectrum.Values.Count > Data.Length)
            {
                try
                {
                    Data = Spectrum.Values.ToList().GetRange(0, Data.Length).ToArray();
                }
                catch (ArgumentException) { }
            }
            else
            {
                Data = new double[Data.Length];
            }
            Visualizer.CTSet(control =>
            {
                control.ChannelData = Data;

                if (control.CheckVariables())
                {
                    control.InvalidateVisual();
                    control.GUIUpdated = false;
                }
            });
        }

        internal void ProcessData(byte[] data, bool silent)
        {
            Spectrum.ProcessData(data, silent);
        }

        internal void ClearPreValues()
        {
            Spectrum.ClearPreValues();
        }

        public string GetOption(string name, string attributeName)
        {
            return this.OptionsRoot.Element(name).Attribute(attributeName).Value;
        }

        private void SetHandlers(bool remove)
        {
            if (remove)
            {
                this.Timer.IsEnabled = false;
                this.Timer.Stop();
                this.Timer = null;
                this.Visualizer?.Dispose();
            }
            else
            {
                this.Timer = new DispatcherTimer()
                {
                    IsEnabled = false,
                    Interval = TimeSpan.FromSeconds(1),
                };
            }
        }

        public SpectrumControl()
        {
            InitializeComponent();
            Visualizer.SpectrumAnalyzer.SpectrumDataChanged += new SpectrumAnalyzer.SpectrumDataChangedEventHandler(this.OnSpectrumDataChanged);
        }
    }
}