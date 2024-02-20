using Core.Main;
using System.Collections.Generic;
using System.Windows.Media;

namespace Core.Application
{
    public class Manager
    {
        public ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public ServerSettings ServerSettings { get; set; } = new ServerSettings();
        public static Dictionary<ThemeDesignation, Theme> Themes { get; set; } = new Dictionary<ThemeDesignation, Theme>()
        {
            { new ThemeDesignation(ThemeType.Default, "стандартная"), new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.Thistle.ToMediaColor()), //верхняя плашка
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleTurquoise.ToMediaColor()), //первый основной цвет
                ThirdColor = new SolidColorBrush(System.Drawing.Color.PaleGreen.ToMediaColor()), //второй основной цвет
                FourthColor = new SolidColorBrush(System.Drawing.Color.MediumPurple.ToMediaColor()) } }, //рамка вокруг окна
            { new ThemeDesignation(ThemeType.DefaultTwo, "стандартная 2"),  new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.Thistle.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleTurquoise.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.NavajoWhite.ToMediaColor()),
                FourthColor =new SolidColorBrush(System.Drawing.Color.MediumPurple.ToMediaColor()) } },
            { new ThemeDesignation(ThemeType.Windows, "оконная"),  new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.CornflowerBlue.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.PaleGoldenrod.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.LightSkyBlue.ToMediaColor()),
                FourthColor =new SolidColorBrush(System.Drawing.Color.DeepSkyBlue.ToMediaColor()) } },
            { new ThemeDesignation(ThemeType.Monochrome, "монохромная"),  new Theme() {
                FirstColor = new SolidColorBrush(System.Drawing.Color.Gray.ToMediaColor()),
                SecondColor = new SolidColorBrush(System.Drawing.Color.LightSlateGray.ToMediaColor()),
                ThirdColor = new SolidColorBrush(System.Drawing.Color.LightGray.ToMediaColor()),
                FourthColor =new SolidColorBrush(System.Drawing.Color.Black.ToMediaColor()) } }
        };
    }

    public class Settings { }


    public struct ThemeDesignation
    {
        public ThemeType ThemeType { get; set; }
        public string ThemeName { get; set; }
        public ThemeDesignation(ThemeType ThemeType, string ThemeName)
        {
            this.ThemeType = ThemeType;
            this.ThemeName = ThemeName;
        }
    }
}