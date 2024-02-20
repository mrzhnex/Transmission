using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace AudioVisualizationLibrary.Classes.Intersection
{
    internal class PolygonIntersection
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Intersection
        {
            public double Key;
            public Point Value;
            public Intersection(double key, Point value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        // Return points where the segment enters and leaves the polygon.
        public static List<Intersection> ClipLineWithPolygon(out bool starts_outside_polygon, System.Windows.Point point1, Point point2, List<Point> polygon_points)
        {
            // Make lists to hold points of
            // intersection and their t values.
            //List<Point> intersections = new List<Point>();
            //List<double> t_values = new List<double>();

            List<Intersection> intersections = new List<Intersection>()
            {
                // Add the segment's starting point.
                new Intersection(0.0, point1),
            };

            //intersections.Add(point1);
            //t_values.Add(0f);

            starts_outside_polygon = !PointIsInPolygon(point1.X, point1.Y, polygon_points);

            // Examine the polygon's edges.
            for (int i1 = 0; i1 < polygon_points.Count; i1++)
            {
                // Get the end points for this edge.
                int i2 = (i1 + 1) % polygon_points.Count;

                // See where the edge intersects the segment.
                FindIntersection(point1, point2,
                    polygon_points[i1], polygon_points[i2],
                    out bool lines_intersect, out bool segments_intersect,
                    out Point intersection, out Point close_p1, out Point close_p2,
                    out double t1, out double t2);

                // See if the segment intersects the edge.
                if (segments_intersect)
                {
                    // See if we need to record this intersection.

                    // Record this intersection.
                    //intersections.Add(intersection);
                    //t_values.Add(t1);

                    intersections.Add(new Intersection(t1, intersection));
                }
            }

            // Add the segment's ending point.
            //intersections.Add(point2);
            //t_values.Add(1f);

            intersections.Add(new Intersection(1.0, point2));

            // Sort the points of intersection by t value.
            /*Point[] intersections_array = intersections.ToArray();
            double[] t_array = t_values.ToArray();
            Array.Sort(t_array, intersections_array);*/

            intersections.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Return the intersections.
            return intersections;
        }
        public static List<Intersection> ClipLineWithPolygon(out bool starts_outside_polygon, System.Windows.Point point1, Point point2, Point[] polygon_points)
        {
            // Make lists to hold points of
            // intersection and their t values.
            //List<Point> intersections = new List<Point>();
            //List<double> t_values = new List<double>();

            List<Intersection> intersections = new List<Intersection>()
            {
                // Add the segment's starting point.
                new Intersection(0.0, point1),
            };

            // Add the segment's starting point.

            starts_outside_polygon = !PointIsInPolygon(point1.X, point1.Y, polygon_points);

            // Examine the polygon's edges.
            for (int i1 = 0; i1 < polygon_points.Length; i1++)
            {
                // Get the end points for this edge.
                int i2 = (i1 + 1) % polygon_points.Length;

                // See where the edge intersects the segment.
                FindIntersection(point1, point2,
                    polygon_points[i1], polygon_points[i2],
                    out bool lines_intersect, out bool segments_intersect,
                    out Point intersection, out Point close_p1, out Point close_p2,
                    out double t1, out double t2);

                // See if the segment intersects the edge.
                if (segments_intersect)
                {
                    // See if we need to record this intersection.

                    // Record this intersection.
                    intersections.Add(new Intersection(t1, intersection));
                }
            }

            // Add the segment's ending point.
            intersections.Add(new Intersection(1.0, point2));


            // Sort the points of intersection by t value.
            intersections.Sort((x, y) => x.Key.CompareTo(y.Key));

            // Return the intersections.
            return intersections;
        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection(Point p1, Point p2, Point p3, Point p4,
            out bool lines_intersect, out bool segments_intersect,
            out Point intersection, out Point close_p1, out Point close_p2,
            out double t1, out double t2)
        {
            // Get the segments' parameters.
            double dx12 = p2.X - p1.X;
            double dy12 = p2.Y - p1.Y;
            double dx34 = p4.X - p3.X;
            double dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);
            t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point(double.NaN, double.NaN);
                close_p1 = new Point(double.NaN, double.NaN);
                close_p2 = new Point(double.NaN, double.NaN);
                t2 = float.PositiveInfinity;
                return;
            }
            lines_intersect = true;

            t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // Find the point of intersection.
            intersection = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0) t1 = 0;
            else if (t1 > 1) t1 = 1;

            if (t2 < 0) t2 = 0;
            else if (t2 > 1) t2 = 1;

            close_p1 = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }


        // Return true if the point is in the polygon.
        public static bool PointIsInPolygon(double X, double Y, List<Point> polygon_points)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = polygon_points.Count - 1;
            double total_angle = GetAngle(
                polygon_points[max_point].X, polygon_points[max_point].Y,
                X, Y,
                polygon_points[0].X, polygon_points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    polygon_points[i].X, polygon_points[i].Y,
                    X, Y,
                    polygon_points[i + 1].X, polygon_points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        public static bool PointIsInPolygon(double X, double Y, Point[] polygon_points)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = polygon_points.Length - 1;
            double total_angle = GetAngle(
                polygon_points[max_point].X, polygon_points[max_point].Y,
                X, Y,
                polygon_points[0].X, polygon_points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    polygon_points[i].X, polygon_points[i].Y,
                    X, Y,
                    polygon_points[i + 1].X, polygon_points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }


        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static double GetAngle(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static double CrossProductLength(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }
    }
}
