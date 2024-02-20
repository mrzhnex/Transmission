using AudioVisualizationLibrary.Classes;
using AudioVisualizationLibrary.Enums;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioVisualizationLibrary.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Audio Visualizer")]
    [Description("Ses görselleştirmesi yapan bir kontroldür.")]
    public partial class AudioVisualizerControl : UserControl
    {
        public VisualizerTypes VisualizerTypeValue = VisualizerTypes.Bar;

        #region Settings - Bar Options

        private int BarCount;

        private double BarWidthValue = 5;

        private double BarHeightValue = 5;
        
        public double FixedVerticalBarCount;
        public double FixedHorizontalBarCount;

        private double SpaceValue = 0;

        public int SectorBarCountValue = 10;

        public int SectorCountValue = 12;

        private double BarSpaceValue = 5;

        private double VerticalBarSpaceValue = 0;

        private double VerticalBarCountValue = 50;
        #endregion

        #region Settings - Style

        private double VisualizerOpacityValue = 1.0;

        private double BarPenSize;
        private double ProgressPenSize;
        private double ProgressLinePenSize;

        private PenLineCap PeakStartLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Peak Start Line Cap")]
        public PenLineCap PeakStartLineCap
        {
            get => this.PeakStartLineCapValue;
            set
            {
                this.PeakStartLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap PeakDashCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Peak Dash Cap")]
        public PenLineCap PeakDashCap
        {
            get => this.PeakDashCapValue;
            set
            {
                this.PeakDashCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap PeakEndLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Peak End Line Cap")]
        public PenLineCap PeakEndLineCap
        {
            get => this.PeakEndLineCapValue;
            set
            {
                this.PeakEndLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private DashStyle PeakDashStyleValue = DashStyles.Solid;

        [Category("Settings - Style"), DisplayName("Peak Dash Style")]
        public DashStyle PeakDashStyle
        {
            get => this.PeakDashStyleValue;
            set
            {
                this.PeakDashStyleValue = value;
                this.GUIUpdated = true;
            }
        }

        private Brush PeakBrushValue = Brushes.YellowGreen;

        [Category("Settings - Style"), DisplayName("Peak Brush")]
        public Brush PeakBrush
        {
            get => this.PeakBrushValue;
            set
            {
                this.PeakBrushValue = value.SetOpacity(this.VisualizerOpacityValue);
                this.PeakBrushValue.TryFreeze();

                this.PeakPen = null;

                this.GUIUpdated = true;
            }
        }

        private Pen PeakPen;

        private PenLineCap BarStartLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Bar Start Line Cap")]
        public PenLineCap BarStartLineCap
        {
            get => this.BarStartLineCapValue;
            set
            {
                this.BarStartLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap BarDashCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Bar Dash Cap")]
        public PenLineCap BarDashCap
        {
            get => this.BarDashCapValue;
            set
            {
                this.BarDashCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap BarEndLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Bar End Line Cap")]
        public PenLineCap BarEndLineCap
        {
            get => this.BarEndLineCapValue;
            set
            {
                this.BarEndLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private DashStyle BarDashStyleValue = DashStyles.Solid;

        [Category("Settings - Style"), DisplayName("Bar Dash Style")]
        public DashStyle BarDashStyle
        {
            get => this.BarDashStyleValue;
            set
            {
                this.BarDashStyleValue = value;
                this.GUIUpdated = true;
            }
        }

        private Pen BarPenValue;
        public Pen BarPen
        {
            get => this.BarPenValue;
            set
            {
                this.BarPenValue = value;
                this.BarPenValue.TryFreeze();
            }
        }

        private Brush BarBrushValue = Brushes.DodgerBlue;

        [Category("Settings - Style"), DisplayName("Bar Brush")]
        public Brush BarBrush
        {
            get => this.BarBrushValue;
            set
            {
                this.BarBrushValue = value.SetOpacity(this.VisualizerOpacityValue);
                this.BarBrushValue.TryFreeze();
                this.BarPen = null;

                this.GUIUpdated = true;
            }
        }

        private PenLineCap ProgressStartLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Progress Start Line Cap")]
        public PenLineCap ProgressStartLineCap
        {
            get => this.ProgressStartLineCapValue;
            set
            {
                this.ProgressStartLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap ProgressDashCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Progress Dash Cap")]
        public PenLineCap ProgressDashCap
        {
            get => this.ProgressDashCapValue;
            set
            {
                this.ProgressDashCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap ProgressEndLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Progress End Line Cap")]
        public PenLineCap ProgressEndLineCap
        {
            get => this.ProgressEndLineCapValue;
            set
            {
                this.ProgressEndLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private DashStyle ProgressDashStyleValue = DashStyles.Solid;

        [Category("Settings - Style"), DisplayName("Progress Dash Style")]
        public DashStyle ProgressDashStyle
        {
            get => this.ProgressDashStyleValue;
            set
            {
                this.ProgressDashStyleValue = value;
                this.GUIUpdated = true;
            }
        }

        private Brush ProgressBrushValue = Brushes.DodgerBlue;

        [Category("Settings - Style"), DisplayName("Progress Brush")]
        public Brush ProgressBrush
        {
            get => this.ProgressBrushValue;
            set
            {
                this.ProgressBrushValue = value.SetOpacity(this.VisualizerOpacityValue);
                this.ProgressBrushValue.TryFreeze();

                this.ProgressPen = this.ProgressPenNormal = null;

                this.GUIUpdated = true;
            }
        }

        private Pen ProgressPenValue;
        public Pen ProgressPen
        {
            get => this.ProgressPenValue;
            set
            {
                this.ProgressPenValue = value;
                this.ProgressPenValue.TryFreeze();
            }
        }

        private Pen ProgressPenNormal;

        private PenLineCap ProgressLineStartLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Progress Line Start Line Cap")]
        public PenLineCap ProgressLineStartLineCap
        {
            get => this.ProgressLineStartLineCapValue;
            set
            {
                this.ProgressLineStartLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private PenLineCap ProgressLineEndLineCapValue = PenLineCap.Flat;

        [Category("Settings - Style"), DisplayName("Progress Line End Line Cap")]
        public PenLineCap ProgressLineEndLineCap
        {
            get => this.ProgressLineEndLineCapValue;
            set
            {
                this.ProgressLineEndLineCapValue = value;
                this.GUIUpdated = true;
            }
        }

        private Brush ProgressLineBrushValue = Brushes.YellowGreen;

        [Category("Settings - Style"), DisplayName("Progress Line Brush")]
        public Brush ProgressLineBrush
        {
            get => this.ProgressLineBrushValue;
            set
            {
                this.ProgressLineBrushValue = value.SetOpacity(this.VisualizerOpacityValue);
                this.ProgressLineBrushValue.TryFreeze();
                this.ProgressLinePen = this.ProgressLinePenNormal = null;

                this.GUIUpdated = true;
            }
        }

        private Pen ProgressLinePenValue;
        public Pen ProgressLinePen
        {
            get => this.ProgressLinePenValue;
            set
            {
                this.ProgressLinePenValue = value;
                this.ProgressLinePenValue.TryFreeze();
            }
        }

        private Pen ProgressLinePenNormal;
        #endregion

        public bool IsDesignMode => DesignerProperties.GetIsInDesignMode(this);

        [Category("Readable"), DisplayName("Spectrum Analyzer Engine"), Description("Analyzer sınıfına erişimi sağlar.")]
        public SpectrumAnalyzer SpectrumAnalyzer { get; }

        #region Private Properties

        private bool GUIUpdatedValue = false;
        public bool GUIUpdated
        {
            get => this.GUIUpdatedValue;
            set
            {
                this.GUIUpdatedValue = value;

                if (value)
                {
                    this.CTSet<AudioVisualizerControl>(c => c.UpdateGUI());
                }
            }
        }

        public double[] ChannelData = new double[120];
        private double[,] MaximumValues;

        public Point Center;

        private readonly DeviceListInfo DeviceList;
        private readonly ProgressInfo ProgressLeft;
        private readonly ProgressInfo ProgressRight;
        #endregion

        public delegate void ValueChangedEventHandler(ProgressInfo info);

        [Category("Visualizer Events"), DisplayName("Value Changed")]
        public event ValueChangedEventHandler ProgressValueChanged;

        public AudioVisualizerControl()
        {
            InitializeComponent();

            this.DeviceList = new DeviceListInfo();

            this.ProgressLeft = new ProgressInfo()
            {
                Minimum = 0,
                Maximum = ushort.MaxValue,
                Direction = Directions.Left,
            };

            this.ProgressRight = new ProgressInfo()
            {
                Minimum = 0,
                Maximum = ushort.MaxValue,
                Direction = Directions.Right,
            };

            this.SpectrumAnalyzer = new SpectrumAnalyzer(this.BarCount, this.ProgressLeft, this.ProgressRight, this, this.DeviceList);
            this.SpectrumAnalyzer.OnInitialized += new SpectrumAnalyzer.InitializedEventHandler(this.OnInitialized);
            this.SpectrumAnalyzer.Enable = true;

            this.Loaded += new RoutedEventHandler(this.OnLoad);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.SetHandlers(false);
            this.GUIUpdated = true;
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.GUIUpdated = true;
        }
        private void SetHandlers(bool remove)
        {
            if (remove)
            {
                if (this.SpectrumAnalyzer != null)
                {
                    this.SpectrumAnalyzer.OnInitialized -= new SpectrumAnalyzer.InitializedEventHandler(this.OnInitialized);

                    this.SpectrumAnalyzer.Dispose();
                }

                this.ProgressLeft.ValueChanged -= new ProgressInfo.ValueChangedEventHandler(this.ProgressChanged);
                this.ProgressRight.ValueChanged -= new ProgressInfo.ValueChangedEventHandler(this.ProgressChanged);

                this.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
            }
            else
            {
                this.ProgressLeft.ValueChanged += new ProgressInfo.ValueChangedEventHandler(this.ProgressChanged);
                this.ProgressRight.ValueChanged += new ProgressInfo.ValueChangedEventHandler(this.ProgressChanged);

                this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.Property.Name)
            {
                case "Width":
                case "ActualWidth":
                case "Height":
                case "ActualHeight":
                    //this.GUIUpdated = true;
                    break;
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            switch(this.VisualizerTypeValue)
            {
                case VisualizerTypes.Bar:
                case VisualizerTypes.BarDouble:
                    this.DrawBarVisualizer(drawingContext);
                    break;
            }
        }

        private void OnInitialized(object sender)
        {
            //MessageBox.Show("On Initialized");
        }
        private void ProgressChanged(ProgressInfo info)
        {
            this.ProgressValueChanged?.Invoke(info);
        }
        private void SetAnalyzerSample(int count)
        {
            if (this.SpectrumAnalyzer != null)
            {
                this.CTSet<AudioVisualizerControl>(control =>
                {
                    control.MaximumValues = new double[2, count];

                    control.SpectrumAnalyzer.Enable = false;
                    control.SpectrumAnalyzer.SampleCount = count;
                    control.SpectrumAnalyzer.Enable = true;

                });
            }
        }
        private void UpdateGUI()
        {
            if (this.GUIUpdated)
            {
                this.CTSet<AudioVisualizerControl>(control =>
                {
                    control.BarCount = this.SectorCountValue * this.SectorBarCountValue;

                    switch (this.VisualizerTypeValue)
                    {
                        case VisualizerTypes.Bar:
                        case VisualizerTypes.BarDouble:
                            {
                                control.Center = new Point(control.ActualWidth / 2.0, control.ActualHeight / 2.0);
                                //control.Radius = Math.Min(control.Center.X, control.Center.Y) * 0.5;
                                control.ProgressLinePenSize = this.SpaceValue > 0 ? this.SpaceValue / 4.0 : 2.0;
                                control.ProgressPenSize = control.ProgressLinePenSize * 2.0;

                                double h = this.VisualizerTypeValue == VisualizerTypes.Bar ? this.ActualHeight : this.Center.Y - (this.SpaceValue / 2.0);

                                control.BarWidthValue = Math.Max(((this.ActualWidth - (this.BarSpaceValue * (this.BarCount + 1))) / this.BarCount), 1);
                                control.FixedHorizontalBarCount = Math.Max((int)((this.ActualWidth - this.BarSpaceValue) / (this.BarWidthValue + this.BarSpaceValue)), 1);

                                control.BarHeightValue = Math.Max(((h - (this.VerticalBarSpaceValue * (this.VerticalBarCountValue + 1))) / this.VerticalBarCountValue), 1);
                                control.FixedVerticalBarCount = Math.Max((int)((h - this.VerticalBarSpaceValue) / (this.BarHeightValue + this.VerticalBarSpaceValue)), 1);

                                control.BarPenSize = 0;

                                break;
                            }
                    }

                    control.SetAnalyzerSample(control.BarCount);

                });

                //kullanılan brushların alpha değerlerini yenile
                //this.SetBrushOpacity();

                this.BarPenValue = DrawingMethods.CreatePen(this.BarBrushValue, this.BarPenSize, this.BarStartLineCapValue, this.BarEndLineCapValue, this.BarDashCapValue, this.BarDashStyleValue);
                
                this.PeakPen = DrawingMethods.CreatePen(this.PeakBrushValue, this.BarPenSize, this.PeakStartLineCapValue, this.PeakEndLineCapValue, this.PeakDashCapValue, this.PeakDashStyleValue);
                this.PeakPen.Freeze();

                this.ProgressPenValue = DrawingMethods.CreatePen(this.ProgressBrushValue, this.ProgressPenSize, this.ProgressStartLineCapValue, this.ProgressEndLineCapValue, this.ProgressDashCapValue, this.ProgressDashStyleValue);
                
                this.ProgressPenNormal = new Pen(this.ProgressBrushValue, this.ProgressPenSize);
                this.ProgressPenNormal.Freeze();

                this.ProgressLinePenValue = DrawingMethods.CreatePen(this.ProgressLineBrushValue, this.ProgressLinePenSize, this.ProgressStartLineCapValue, this.ProgressEndLineCapValue, this.ProgressDashCapValue, this.ProgressDashStyleValue);
                
                this.ProgressLinePenNormal = new Pen(this.ProgressLineBrushValue, this.ProgressLinePenSize);
                this.ProgressLinePenNormal.Freeze();

                //Multi Color Options

            }
        }

        public void Dispose()
        {
            this.SetHandlers(true);

            this.PeakPen = null;
            this.BarPen = null;

            this.ProgressPen = null;
            this.ProgressLinePen = null;
        }
        private void DrawBarVisualizer(DrawingContext context)
        {
            if (!this.CheckVariables()) return;

            double yCoord, xCoord, data, peakPos;
            int barPos;

            Rect rect;

            Pen pen;
            Brush brush;
            
            for (int row = 0; row < this.FixedVerticalBarCount; row++)
            {
                yCoord = this.VerticalBarSpaceValue + (this.BarHeightValue * row) + (this.VerticalBarSpaceValue * row) + 1;

                for (int col = 0; col < this.FixedHorizontalBarCount; col++)
                {
                    data = this.ChannelData[col];
                    barPos = (int)((this.FixedVerticalBarCount * data / this.SpectrumAnalyzer.MaxValue) * .7);
                    xCoord = this.BarSpaceValue + (this.BarWidthValue * col) + (this.BarSpaceValue * col) + 1;
                    peakPos = barPos;

                    //maksimum üst noktayı karşılaştır ve güncelle
                    if (this.MaximumValues[0, col] < peakPos)
                        this.MaximumValues[0, col] = peakPos;

                    if (this.MaximumValues[0, col] > peakPos)
                        peakPos = this.MaximumValues[0, col];
                    else
                        this.MaximumValues[0, col] = peakPos;

                    brush = this.BarBrushValue;
                    pen = this.BarPenValue;

                    //draw bars
                    if (row - 1 < barPos || row == 0)
                    {
                        rect = new Rect()
                        {
                            Location = new Point(xCoord, this.ActualHeight - (yCoord + this.BarHeightValue)),
                            Width = this.BarWidthValue,
                            Height = this.BarHeightValue,
                        };
                        context.DrawRectangle(brush, pen, rect);
                    }
                }
            }
        }
        public bool CheckVariables()
        {
            bool check = false;

            switch(this.VisualizerTypeValue)
            {
                case VisualizerTypes.Bar:
                case VisualizerTypes.BarDouble:
                    check = this.ChannelData != null && this.ChannelData.Length > 2 && this.BarCount > 2 && this.BarCount == this.ChannelData.Length;
                    break;
            }

            return check;
        }
    }
}