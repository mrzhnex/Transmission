using System;
using System.Collections.Generic;
using System.Windows;

namespace AudioVisualizationLibrary.Classes
{
    public static partial class MathF
    {
        //https://www.reddit.com/r/dailyprogrammer/comments/1tixzk/122313_challenge_146_easy_polygon_perimeter/
        //https://www.mathsisfun.com/geometry/regular-polygons.html
        internal class RegularPolygon
        {

            public int iNumberOfSides;
            public double dSideLength;
            public double dCircumRadius;
            public double dApothem;
            public RegularPolygon(int iNumberOfSides, double dCircumRadius)
            {
                this.iNumberOfSides = iNumberOfSides;
                this.dCircumRadius = dCircumRadius;
                this.dSideLength = SideLengthFromRadius(iNumberOfSides, dCircumRadius);
                this.dApothem = ApothemFromRadius(iNumberOfSides, dCircumRadius);
            }

            public static double ExteriorAngle(double sides)
            {
                return 360.0 / sides;
            }

            internal double InteriorAngle(double sides)
            {
                //(sides-2) * 180f / sides;
                return 180.0 - ExteriorAngle(sides);
            }

            /// <summary>
            /// Perimeter of polygon
            /// </summary>
            /// <param name="sides"></param>
            /// <returns></returns>
            public double Area(double sides)
            {
                double perimeter = sides * this.dSideLength;
                return perimeter * this.dApothem / 2.0;
            }

            public double SmallTriangleArea(double sides)
            {
                return (1.0 / 2.0) * Math.Pow(this.dApothem, 2.0) * Math.Tan(Math.PI / sides);
            }

            public double SmallTriangleSideLength(double sides)
            {
                return 2.0 * this.dApothem * Math.Tan(Math.PI / sides);
            }

            public static double SideLengthFromRadius(double iNumberOfSides, double dCircumRadius)
            {
                return dCircumRadius * 2.0 * Math.Sin(Math.PI / iNumberOfSides);
            }
            public static double ApothemFromRadius(double iNumberOfSides, double dCircumRadius)
            {
                return dCircumRadius * 2 * Math.Cos(Math.PI / iNumberOfSides);
            }

            public static Point[] GetPolygonPoints(int sides, double radius, double startingAngle, Point center)
            {
                if (sides < 3)
                    throw new ArgumentException("Polygon must have 3 sides or more.");

                Point[] points = new Point[sides + 1];

                double step = 360.0 / sides;
                double angle = startingAngle; //starting angle

                /*for (double i = startingAngle; i < startingAngle + 360.0; i += step) //go in a circle
                {
                    points[index++] = MathF.GetPoint(center, radius, angle);
                    angle += step;

                }*/

                for (int i = 0; i < sides; i++)
                {
                    points[i] = MathF.GetPoint(center, radius, angle);
                    angle += step;
                }

                points[points.Length - 1] = points[0];

                return points;
            }
        }
    }
}
