using System.Windows.Media;

namespace AudioVisualizationLibrary.Classes
{
    public static class DrawingMethods
    {
        public static Pen CreatePen(Brush brush, double size, PenLineCap startLineCap, PenLineCap endLineCap, PenLineCap dashCap, DashStyle dashStyle)
        {
            Pen pen = new Pen(brush, size) { StartLineCap = startLineCap, EndLineCap = endLineCap, DashCap = dashCap, DashStyle = dashStyle };

            if (pen.CanFreeze)
                pen.Freeze();

            return pen;
        }

        public static Geometry CreateGeometry(this DrawingGroup drawingGroup)
        {
            var geometry = new GeometryGroup();

            foreach (var drawing in drawingGroup.Children)
            {
                if (drawing is GeometryDrawing gmd)
                {
                    geometry.Children.Add(gmd.Geometry);
                }
                else if (drawing is GlyphRunDrawing grd)
                {
                    geometry.Children.Add(grd.GlyphRun.BuildGeometry());
                }
                else if (drawing is DrawingGroup dg)
                {
                    geometry.Children.Add(CreateGeometry(dg));
                }
            }

            geometry.Transform = drawingGroup.Transform;

            return geometry;
        }
    }
}
