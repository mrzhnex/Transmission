using System;
using System.Windows.Media;

namespace AudioVisualizationLibrary.Classes.ColorLibrary
{
    public static class ColorTools
    {
        public static Color ToColor(this Brush brush) => ((SolidColorBrush)brush).Color;
        public static Color ToColor(this System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
        public static Brush ToBrush(this Color color) => new SolidColorBrush(color);
        public static double GetHue(this System.Windows.Media.Color c) => Convert.ToDouble(System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue());
        public static double GetBrightness(this System.Windows.Media.Color c) => Convert.ToDouble(System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetBrightness());
        public static double GetSaturation(this System.Windows.Media.Color c) => Convert.ToDouble(System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetSaturation());

        /// <summary>
        /// Belirtilen rengin saydamlık ayarını değiştirir.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="alpha">0 - 255 arasında bir değer giriniz.</param>
        /// <returns></returns>
        public static Color SetAlpha(this Color c, byte alpha) => Color.FromArgb(alpha, c.R, c.G, c.B);

        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/26a68ffd-bd51-4a72-8eda-d1dcff556f1d/make-color-lighter?forum=wpf
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static Color InterpolateColors(Color color1, Color color2, double percentage)
        {
            double a1 = color1.A / 255.0, r1 = color1.R / 255.0, g1 = color1.G / 255.0, b1 = color1.B / 255.0;
            double a2 = color2.A / 255.0, r2 = color2.R / 255.0, g2 = color2.G / 255.0, b2 = color2.B / 255.0;

            byte a3 = Convert.ToByte((a1 + (a2 - a1) * percentage) * 255);
            byte r3 = Convert.ToByte((r1 + (r2 - r1) * percentage) * 255);
            byte g3 = Convert.ToByte((g1 + (g2 - g1) * percentage) * 255);
            byte b3 = Convert.ToByte((b1 + (b2 - b1) * percentage) * 255);

            return Color.FromArgb(a3, r3, g3, b3);
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// https://www.pvladov.com/2012/09/make-color-lighter-or-darker.html
        /// </summary>
        /// <param name="color">Belirtilecek olan renk</param>
        /// <param name="correctionFactor">-1 ile 1 arasında bir değer giriniz. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color SetBrightness(this Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }


            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

    }
}
