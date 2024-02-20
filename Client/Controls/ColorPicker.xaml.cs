using AudioVisualizationLibrary.Classes.ColorLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Controls
{
    /// <summary>
    /// ColorPicker.xaml etkileşim mantığı
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        internal delegate void ColorSelectionChangedEventHandler(ColorInfo info);
        internal event ColorSelectionChangedEventHandler ColorSelectionChanged;

        internal static List<ColorPicker.ColorInfo> Colors;
        internal ColorInfo SelectedColor;

        private readonly int ColumnCount = 10;
        private readonly int RowHeight = 30;
        private readonly int Space = 5;

        private bool IsSubControlValue = false;
        public bool IsSubControl
        {
            get => this.IsSubControlValue;
            set
            {
                this.IsSubControlValue = value;

                if (value)
                {
                    this.CancelButton.Visibility = Visibility.Hidden;
                    this.OKButton.Visibility = Visibility.Hidden;
                    this.MainGrid.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Pixel);
                }
                else
                {
                    this.CancelButton.Visibility = Visibility.Visible;
                    this.OKButton.Visibility = Visibility.Visible;
                    this.MainGrid.RowDefinitions[2].Height = new GridLength(30, GridUnitType.Pixel);
                }
            }
        }
        public ColorPicker()
        {
            InitializeComponent();


            Colors = typeof(System.Windows.Media.Colors).GetProperties()
                            .Select(f => new ColorPicker.ColorInfo(f.Name, (System.Windows.Media.Color)f.GetValue(null, null)))
                            .OrderByDescending(f => f.Hue)
                            .ThenByDescending(f => f.Saturation)
                            .ThenByDescending(f => f.Brightness)
                            .ToList();

            this.Loaded += new RoutedEventHandler(this.OnLoad);
            this.OKButton.Click += new RoutedEventHandler(this.OKClick);
            this.CancelButton.Click += new RoutedEventHandler(this.CancelClick);
        }


        private void OKClick(object sender, RoutedEventArgs e)
        {
            this.ColorSelectionChanged?.Invoke(this.SelectedColor);
            this.Hide();
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.LoadColors(Colors);
            this.Hide();
        }

        private void LoadColors(List<ColorInfo> liste)
        {
            this.ColorTableGrid.Children.Clear();
            this.ColorTableGrid.ColumnDefinitions.Clear();
            this.ColorTableGrid.RowDefinitions.Clear();

            this.SV.Padding = new Thickness(this.Space / 2);

            int index = 0;
            int faktor = (liste.Count % this.ColumnCount == 0) ? 0 : 1;
            int rowCount = (liste.Count / this.ColumnCount) + faktor;

            double width = (this.SV.ActualWidth - (SystemParameters.ScrollWidth + this.Space)) / this.ColumnCount;
            double height = this.RowHeight;// this.LV.Height / rowCount;

            ColorInfo info;
            Button buton;

            for (int i = 0; i < this.ColumnCount; i++)
                this.ColorTableGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, GridUnitType.Pixel) });

            for (int row = 0; row < rowCount; row++)
            {
                this.ColorTableGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(height, GridUnitType.Pixel) });

                for (int column = 0; column < this.ColumnCount; column++)
                {
                    if (index < liste.Count)
                    {
                        info = liste[index];

                        buton = new Button()
                        {
                            Padding = new Thickness(0),
                            Margin = new Thickness(0),
                            Width = width - this.Space > 0 ? width - this.Space : width,
                            Height = height - this.Space > 0 ? height - this.Space : height,
                            Background = new SolidColorBrush(info.Color),
                            ToolTip = info.Name,
                            Foreground = info.Brightness > .5 ? Brushes.Black : Brushes.White,
                            Tag = info,
                            BorderBrush = Brushes.White,
                            BorderThickness = new Thickness(1.0),
                        };

                        buton.Click += new RoutedEventHandler(this.SelectColorClick);

                        this.ColorTableGrid.Children.Add(buton);

                        Grid.SetColumn(buton, column);
                        Grid.SetRow(buton, row);

                        index++;
                    }
                }
            }
        }
        private void SelectColorClick(object sender, RoutedEventArgs e)
        {
            Button buton = (Button)sender;

            if (buton.Tag is ColorInfo info)
                this.SelectedColor = info;

            this.SelectedColorButton.Background = new SolidColorBrush(this.SelectedColor.Color);
            this.SelectedColorTextbox.Text = this.SelectedColor.Name;

            if (this.IsSubControlValue)
                this.ColorSelectionChanged?.Invoke(this.SelectedColor);
        }
        public void Show() => this.Visibility = Visibility.Visible;
        public void Hide() => this.Visibility = Visibility.Hidden;
        public void ToggleVisibility()
        {
            if (this.IsVisible)
                this.Hide();
            else
                this.Show();
        }

        public void SetColor(Brush brush)
        {
            this.SelectedColor = Colors.First(f => f.Color == brush.ToColor());

            this.SelectedColorButton.Background = new SolidColorBrush(this.SelectedColor.Color);
            this.SelectedColorTextbox.Text = this.SelectedColor.Name;

            this.ColorSelectionChanged?.Invoke(this.SelectedColor);
        }

        public static void SetButtonColors(Button button, Brush brush)
        {
            ColorInfo info = Colors.First(f => f.Color == brush.ToColor());

            button.Foreground = info.Brightness < .5 ? Brushes.White : Brushes.Black;
            button.Background = brush;
            button.ToolTip = info.Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ColorInfo
        {
            public string Name;

            private Color ColorValue;
            public Color Color
            {
                get => this.ColorValue;
                set
                {
                    this.ColorValue = value;

                    this.Brightness = value.GetBrightness();
                    this.Hue = value.GetHue();
                    this.Saturation = value.GetSaturation();
                }
            }

            public double Brightness;
            public double Hue;
            public double Saturation;
            public ColorInfo(string name, Color color)
            {
                this.Name = name;
                this.ColorValue = color;

                this.Brightness = color.GetBrightness();
                this.Hue = color.GetHue();
                this.Saturation = color.GetSaturation();
            }
        }
    }
}
