using AudioVisualizationLibrary.Classes.ColorLibrary;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server.Controls
{
    /// <summary>
    /// PenEditor.xaml etkileşim mantığı
    /// </summary>
    public partial class PenEditor : UserControl
    {
        internal static DashStyleInfo[] Dashes = new[]
            {
                new DashStyleInfo("Dash",  DashStyles.Dash),
                new DashStyleInfo("DashDot", DashStyles.DashDot),
                new DashStyleInfo("DashDotDot", DashStyles.DashDotDot),
                new DashStyleInfo("Dot", DashStyles.Dot),
                new DashStyleInfo("Solid", DashStyles.Solid)
            };

        public PenLineCap StartLineCap;
        public PenLineCap EndLineCap;
        public PenLineCap DashCap;
        internal DashStyleInfo DashStyle;
        internal ColorPicker.ColorInfo Color;

        internal delegate void SetStyleEventHandler(ColorPicker.ColorInfo Color, PenLineCap StartLineCap, PenLineCap EndLineCap, PenLineCap DashCap, DashStyleInfo DashStyle);
        internal event SetStyleEventHandler OnSetStyle;
        public PenEditor()
        {
            InitializeComponent();

            this.PenColorPicker.IsSubControl = true;
            this.PenColorPicker.Background = Brushes.Transparent;

            this.Loaded += new RoutedEventHandler(this.OnLoad);
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.SetHandlers(false);
        }

        private void ColorSelectionChanged(ColorPicker.ColorInfo info)
        {
            this.Color = info;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            this.OnSetStyle?.Invoke(this.Color, this.StartLineCap, this.EndLineCap, this.DashCap, this.DashStyle);
            this.Hide();
        }

        public void Show() => this.Visibility = Visibility.Visible;
        public void Hide() => this.Visibility = Visibility.Hidden;
        public void ToggleVisibility()
        {
            if (this.IsVisible)
                this.Hide();
            else
            {
                this.Show();
                this.PenColorPicker.Show();
            }
        }

        internal static DashStyleInfo GetDashStyle(string style) => Dashes.First(f => f.Name == style);
        internal static DashStyleInfo GetDashStyle(DashStyle style) => Dashes.First(f => f.Style == style);

        public void SetStyle(Button buton, Brush brush, PenLineCap startLineCap, PenLineCap endLineCap, PenLineCap dashCap, DashStyle dashStyle)
        {
            this.Color = this.PenColorPicker.SelectedColor = ColorPicker.Colors.First(f => f.Color == brush.ToColor());

            this.PenColorPicker.SetColor(brush);

            this.StartLineCap = startLineCap;
            this.EndLineCap = endLineCap;
            this.DashCap = dashCap;
            this.DashStyle = GetDashStyle(dashStyle);

            ColorPicker.SetButtonColors(buton, brush);
        }
        private void DashStyleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.SelectedItem is String item)
            {
                this.DashStyle = GetDashStyle(item);
            }
        }
        private void DashCapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.SelectedItem is String item)
            {
                this.DashCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), item);
            }
        }
        private void EndLineCapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.SelectedItem is String item)
            {
                this.EndLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), item);
            }
        }
        private void StartLineCapSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;

            if (combo.SelectedItem is String item)
            {
                this.StartLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), item);
            }
        }
        private void SetHandlers(bool remove)
        {
            if (remove)
            {
                this.PenColorPicker.ColorSelectionChanged -= new ColorPicker.ColorSelectionChangedEventHandler(this.ColorSelectionChanged);
                this.ComboStartLineCap.SelectionChanged -= new SelectionChangedEventHandler(this.StartLineCapSelectionChanged);
                this.ComboEndLineCap.SelectionChanged -= new SelectionChangedEventHandler(this.EndLineCapSelectionChanged);
                this.ComboDashCap.SelectionChanged -= new SelectionChangedEventHandler(this.DashCapSelectionChanged);
                this.ComboDashStyle.SelectionChanged -= new SelectionChangedEventHandler(this.DashStyleSelectionChanged);

                this.OKButton.Click -= new RoutedEventHandler(this.OKClick);
                this.CancelButton.Click -= new RoutedEventHandler(this.CancelClick);

            }
            else
            {
                PenLineCap[] caps = Enum.GetValues(typeof(PenLineCap)).Cast<PenLineCap>().ToArray();

                for (int i = 0; i < caps.Length; i++)
                {
                    PenLineCap lineCap = caps[i];

                    this.ComboStartLineCap.Items.Add(lineCap.ToString());
                    this.ComboEndLineCap.Items.Add(lineCap.ToString());
                    this.ComboDashCap.Items.Add(lineCap.ToString());

                    if (this.StartLineCap == lineCap)
                        this.ComboStartLineCap.SelectedIndex = i;
                    if (this.DashCap == lineCap)
                        this.ComboDashCap.SelectedIndex = i;
                    if (this.EndLineCap == lineCap)
                        this.ComboEndLineCap.SelectedIndex = i;
                }

                for (int i = 0; i < Dashes.Length; i++)
                {
                    DashStyleInfo style = Dashes[i];

                    this.ComboDashStyle.Items.Add(style.Name);

                    if (this.DashStyle != default && this.DashStyle.Name == style.Name)
                        this.ComboDashStyle.SelectedIndex = i;
                }

                this.PenColorPicker.ColorSelectionChanged += new ColorPicker.ColorSelectionChangedEventHandler(this.ColorSelectionChanged);
                this.ComboStartLineCap.SelectionChanged += new SelectionChangedEventHandler(this.StartLineCapSelectionChanged);
                this.ComboEndLineCap.SelectionChanged += new SelectionChangedEventHandler(this.EndLineCapSelectionChanged);
                this.ComboDashCap.SelectionChanged += new SelectionChangedEventHandler(this.DashCapSelectionChanged);
                this.ComboDashStyle.SelectionChanged += new SelectionChangedEventHandler(this.DashStyleSelectionChanged);

                this.OKButton.Click += new RoutedEventHandler(this.OKClick);
                this.CancelButton.Click += new RoutedEventHandler(this.CancelClick);

                this.Hide();

            }
        }
        public void OnClose()
        {
            this.SetHandlers(true);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DashStyleInfo
        {
            public string Name;
            public DashStyle Style;
            public DashStyleInfo(string name, DashStyle style)
            {
                this.Name = name;
                this.Style = style;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(DashStyleInfo left, DashStyleInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(DashStyleInfo left, DashStyleInfo right)
            {
                return !(left == right);
            }
        }

    }
}
