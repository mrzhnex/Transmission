using System;
using System.Windows;
using System.Windows.Media;

namespace AudioVisualizationLibrary.Classes
{
    public static class Extensions
    {
        #region Cross Thread Set
        public static void CTSet<T>(this T kontrol, Action<T> action) where T : UIElement
        {
            if (kontrol == null) return;

            if (!kontrol.Dispatcher.CheckAccess())
                //if (kontrol.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
                kontrol.Dispatcher.Invoke(new Action<T, Action<T>>(CTSet), new object[] { kontrol, action });
            else
                action(kontrol);
        }

        public static void DrwSet<T>(this T kontrol, Action<T> action) where T : System.Windows.Media.DrawingContext
        {
            if (kontrol == null) return;

            if (!kontrol.Dispatcher.CheckAccess())
                kontrol.Dispatcher.Invoke(new Action<T, Action<T>>(DrwSet), new object[] { kontrol, action });
            else
                action(kontrol);
        }
        #endregion
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            do
            {
                if (parent is T matchedParent)
                    return matchedParent;

                parent = VisualTreeHelper.GetParent(parent);
            }

            while (parent != null);

            return null;
        }

        #region Color Extensions
        public static Brush SetOpacity(this Brush brush, double opacity)
        {
            if (brush == null) return brush;

            Brush b;

            bool kontrol = brush.Opacity == opacity;

            if (!kontrol)
            {
                b = brush.Clone();
                b.Opacity = opacity;
            }
            else
                b = brush;

            return b;
        }
        public static ImageBrush SetOpacity(this ImageBrush brush, double opacity)
        {
            if (brush == null) return brush;

            ImageBrush b;

            bool kontrol = brush.Opacity == opacity;

            if (!kontrol)
            {
                b = brush.Clone();
                b.Opacity = opacity;
            }
            else
                b = brush;

            return b;
        }
        public static VisualBrush SetOpacity(this VisualBrush brush, double opacity)
        {
            if (brush == null) return brush;

            VisualBrush b;

            bool kontrol = brush.Opacity == opacity;

            if (!kontrol)
            {
                b = brush.Clone();
                b.Opacity = opacity;
            }
            else
                b = brush;

            return b;
        }
        public static void TryFreeze(this Brush brush)
        {
            if (brush == null) return;

            if (brush.CanFreeze)
                brush.Freeze();
        }
        public static void TryFreeze(this ImageBrush brush)
        {
            if (brush == null) return;

            if (brush.CanFreeze == true)
                brush.Freeze();
        }
        public static void TryFreeze(this Pen pen)
        {
            if (pen == null) return;

            if (pen.CanFreeze == true)
                pen.Freeze();
        }
        #endregion

    }
}
