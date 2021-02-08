using Core.Main;
using System.ComponentModel;
using System.Windows.Media;

namespace Core.Application
{
    public class Theme : INotifyPropertyChanged
    {
        public static Theme Default { get; set; } = new Theme()
        {
            FirstColor = new SolidColorBrush(System.Drawing.Color.Thistle.ToMediaColor()),
            SecondColor = new SolidColorBrush(System.Drawing.Color.PaleTurquoise.ToMediaColor()),
            ThirdColor = new SolidColorBrush(System.Drawing.Color.PaleGreen.ToMediaColor()),
            FourthColor = new SolidColorBrush(System.Drawing.Color.MediumPurple.ToMediaColor())
        };

        public SolidColorBrush FirstColor
        {
            get { return firstColor; }
            set
            {
                firstColor = value;
                OnPropertyChanged(nameof(FirstColor));
            }
        }
        public SolidColorBrush SecondColor
        {
            get { return secondColor; }
            set
            {
                secondColor = value;
                OnPropertyChanged(nameof(SecondColor));
            }
        }
        public SolidColorBrush ThirdColor
        {
            get { return thirdColor; }
            set
            {
                thirdColor = value;
                OnPropertyChanged(nameof(ThirdColor));
            }
        }
        public SolidColorBrush FourthColor
        {
            get { return fourthColor; }
            set
            {
                fourthColor = value;
                OnPropertyChanged(nameof(FourthColor));
            }
        }

        private SolidColorBrush firstColor { get; set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private SolidColorBrush secondColor { get; set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private SolidColorBrush thirdColor { get; set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private SolidColorBrush fourthColor { get; set; } = new SolidColorBrush(Color.FromRgb(0, 0, 0));


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Change(Theme theme)
        {
            FirstColor = theme.FirstColor;
            SecondColor = theme.SecondColor;
            ThirdColor = theme.ThirdColor;
            FourthColor = theme.FourthColor;
        }
    }

    public enum ThemeType
    {
        Default, Windows, Monochrome, DefaultTwo
    }
}