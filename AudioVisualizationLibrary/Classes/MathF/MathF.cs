using AudioVisualizationLibrary.Structures;
using System;
using System.Windows;

namespace AudioVisualizationLibrary.Classes
{
    public static partial class MathF
    {
        #region Sine Wave Methods
        public static double Sine(double x, double t)
        {
            return Math.Sin(Math.PI * (x + t));
        }
        public static Vector3 SineToPoint(double x, double z, double t)
        {
            return new Vector3()
            {
                X = x,
                Y = Math.Sin(Math.PI * (x + t)),
                Z = z,
            };
        }
        public static double Sine2D(double x, double z, double t)
        {
            double y = Math.Sin(Math.PI * (x + t));
            y += Math.Sin(Math.PI * (z + t));
            y *= 0.5;
            return y;
        }

        public static Vector3 Sine2DToPoint(double x, double z, double t)
        {
            Vector3 p = new Vector3()
            {
                X = x,
                Y = Math.Sin(Math.PI * (x + t)),
            };

            p.Y += Math.Sin(Math.PI * (z + t));
            p.Y *= 0.5;

            p.Z = z;

            return p;
        }
        /// <summary>
        /// https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/
        /// </summary>
        /// <param name="x"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double MultiSine(double x, double t)
        {
            double y = Math.Sin(Math.PI * (x + t));
            y += Math.Sin(2.0 * Math.PI * (x + 2.0 * t)) / 2.0;
            y *= 2.0 / 3.0;
            return y;
        }
        public static Vector3 MultiSineToPoint(double x, double z, double t)
        {
            Vector3 p = new Vector3()
            {
                X = x,
                Y = Math.Sin(Math.PI * (x + t)),
            };

            p.Y += Math.Sin(2.0 * Math.PI * (x + 2.0 * t)) / 2.0;
            p.Y *= 2.0 / 3.0;
            p.Z = z;

            return p;
        }
        public static double MultiSine2D(double x, double z, double t)
        {
            double y = 4.0 * Math.Sin(Math.PI * (x + z + t * 0.5));
            y += Math.Sin(Math.PI * (x + t));
            y += Math.Sin(2.0 * Math.PI * (z + 2f * t)) * 0.5;
            y *= 1.0 / 5.5;
            return y;
        }

        public static Vector3 MultiSine2DToPoint(double x, double z, double t)
        {
            Vector3 p = new Vector3()
            {
                X = x,
                Y = 4.0 * Math.Sin(Math.PI * (x + z + t / 2.0)),
            };

            p.Y += Math.Sin(Math.PI * (x + t));
            p.Y += Math.Sin(2f * Math.PI * (z + 2f * t)) * 0.5f;
            p.Y *= 1.0 / 5.5;
            p.Z = z;

            return p;
        }

        /// <summary>
        /// https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/
        /// </summary>
        /// <param name="x"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Wave(double x, double t)
        {
            return Math.Sin(Math.PI * (x + t));
        }
        public static double Ripple(double x, double z)
        {
            double d = Math.Sqrt(x * x + z * z);
            double y = d;
            return y;
        }
        public static Vector3 RippleToPoint(double x, double z, double t)
        {
            Vector3 p = new Vector3();

            double d = Math.Sqrt(x * x + z * z);

            p.X = x;
            p.Y = Math.Sin(Math.PI * (4.0 * d - t));
            p.Y /= (1.0 + 10.0 * d);
            p.Z = z;

            return p;
        }

        public static Vector3 Sphere(double u, double v)
        {
            Vector3 p = new Vector3();
            
            double r = Math.Cos(Math.PI * 0.5 * v);

            p.X = r * Math.Sin(Math.PI * u);
            p.Y = v;
            p.Z = r * Math.Cos(Math.PI * u);

            return p;
        }

        public static Vector3 Torus(double u, double v)
        {
            Vector3 p = new Vector3();

            double s = Math.Cos(Math.PI * 0.5 * v);
            
            p.X = s * Math.Sin(Math.PI * u);
            p.Y = Math.Sin(Math.PI * 0.5 * v);
            p.Z = s * Math.Cos(Math.PI * u);

            return p;
        }
        #endregion

        #region Unity 
        // Loops the value t, so that it is never larger than length and never smaller than 0.
        public static double Repeat(double t, double length)
        {
            return Clamp(t - Math.Floor(t / length) * length, 0.0, length);
        }

        // PingPongs the value t, so that it is never larger than length and never smaller than 0.
        public static double PingPong(double t, double length)
        {
            t = Repeat(t, length * 2.0);
            return length - Math.Abs(t - length);
        }

        // Clamps a value between a minimum float and maximum float value.
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        #endregion

        #region Radians To Degrees - Degrees To Radians
        /// <summary>
        /// 360 derecelik açı değerini Math.PI açısına dönüştürür.
        /// </summary>
        /// <param name="angle">0- 360 değer aralığındaki değerler ile çalışır.</param>
        /// <returns></returns>
        public static double ToRadians(double angle) => (Math.PI * angle / 180.0);

        /// <summary>
        /// Math.PI ile oluşturulan açıları normal açıya dönüştürür.
        /// </summary>
        /// <param name="angle">0 - 6.28 değer aralığındaki değerler ile çalışır.</param>
        /// <returns></returns>
        public static double ToDegrees(double radians) => radians * (180.0 / Math.PI);
        #endregion

        #region Hesaplama Metodları
        /// <summary>
        /// https://www.geeksforgeeks.org/program-find-mid-point-line/
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static Point GetLineMidPoint(double x1, double x2, double y1, double y2) => new Point((x1 + x2) / 2, (y1 + y2) / 2);

        /// <summary>
        /// https://www.geeksforgeeks.org/find-end-point-line-given-one-end-mid/
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Point GetLineEndPoint(double x1, double y1, double m1, double m2)
        {
            // find end point for x cordinates 
            double x2 = (2 * m1 - x1);

            // find end point for y cordinates 
            double y2 = (2 * m2 - y1);

            return new Point(x2, y2);
        }
        public static double GetDistance(double x1, double y1, double x2, double y2) => Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        public static Point GetPoint(Point center, double radius, double angle)
        {
            var radAngle = angle * Math.PI / 180.0;

            var x = center.X + radius * Math.Sin(radAngle);
            var y = center.Y - radius * Math.Cos(radAngle);

            return new Point(x, y);
        }

        public static double Map(double num, double in_min, double in_max, double out_min, double out_max)
        {
            return (num - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        /// <summary>
        /// Bir çemberin içindeki kare ya da dikdörtgen alanı bulur
        /// </summary>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        /// <returns></returns>
        public static Size GetCircleInnerRectangle(double radiusX, double radiusY)
        {
            return new Size((radiusX * 2.0 * Math.Cos(Math.PI / 4.0)), (radiusY * 2.0 * Math.Cos(Math.PI / 4.0)));
        }
        #endregion

    }
}
