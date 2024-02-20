using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AudioVisualizationLibrary.Controls
{
    public class TextOnPathControl : FrameworkElement
    {
        private readonly List<FormattedText> _formattedChars = new List<FormattedText>();
        private readonly VisualCollection _visualChildren;

        private Rect _boundingRect;
        private double _pathLength;

        public PathFigure PathFigure
        {
            get { return (PathFigure)GetValue(PathFigureProperty); }
            set { SetValue(PathFigureProperty, value); }
        }

        public static readonly DependencyProperty PathFigureProperty =
            DependencyProperty.Register("PathFigure", typeof(PathFigure), typeof(TextOnPathControl), new FrameworkPropertyMetadata(OnPathFigureChanged));

        private static void OnPathFigureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = (TextOnPathControl)obj;
            element._pathLength = element.GetPathFigureLength(element.PathFigure);
            element.TransformVisualChildren();
        }

        public TextOnPathControl()
        {
            _boundingRect = new Rect();
            _visualChildren = new VisualCollection(this);
        }

        #region overrides

        protected override int VisualChildrenCount
        {
            get
            {
                return _visualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visualChildren.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _visualChildren[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return (Size)_boundingRect.BottomRight;
        }

        #endregion

        private double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            bool isAlreadyFlattened = true;

            foreach (PathSegment pathSegment in pathFigure.Segments)
            {
                if (!(pathSegment is PolyLineSegment) && !(pathSegment is LineSegment))
                {
                    isAlreadyFlattened = false;
                    break;
                }
            }

            PathFigure pathFigureFlattened = isAlreadyFlattened ? pathFigure : pathFigure.GetFlattenedPathFigure();
            double length = 0;
            Point pt1 = pathFigureFlattened.StartPoint;

            foreach (PathSegment pathSegment in pathFigureFlattened.Segments)
            {
                if (pathSegment is LineSegment)
                {
                    Point pt2 = (pathSegment as LineSegment).Point;
                    length += (pt2 - pt1).Length;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment)
                {
                    PointCollection pointCollection = (pathSegment as PolyLineSegment).Points;
                    foreach (Point pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        pt1 = pt2;
                    }
                }
            }
            return length;
        }

        private void TransformVisualChildren()
        {
            _boundingRect = new Rect();

            if (_pathLength == 0)
                return;

            if (_formattedChars.Count != _visualChildren.Count)
                return;

            var scalingFactor = _pathLength;

            double progress = 0;



            var pathGeometry = new PathGeometry(new[] { PathFigure });
            _boundingRect = new Rect();

            for (int index = 0; index < _visualChildren.Count; index++)
            {
                FormattedText formText = _formattedChars[index];

                double width = scalingFactor * formText.WidthIncludingTrailingWhitespace;
                double baseline = scalingFactor * formText.Baseline;

                progress += width / 2 / _pathLength;

                pathGeometry.GetPointAtFractionLength(progress, out Point point, out Point tangent);

                var drawingVisual = _visualChildren[index] as DrawingVisual;
                var transformGroup = drawingVisual.Transform as TransformGroup;
                var scaleTransform = transformGroup.Children[0] as ScaleTransform;
                var rotateTransform = transformGroup.Children[1] as RotateTransform;
                var translateTransform = transformGroup.Children[2] as TranslateTransform;

                scaleTransform.ScaleX = scalingFactor;
                scaleTransform.ScaleY = scalingFactor;
                rotateTransform.Angle = Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;
                rotateTransform.CenterX = width / 2;
                rotateTransform.CenterY = baseline;
                translateTransform.X = point.X - width / 2;
                translateTransform.Y = point.Y - baseline;

                Rect rect = drawingVisual.ContentBounds;
                rect.Transform(transformGroup.Value);
                _boundingRect.Union(rect);

                progress += width / 2 / _pathLength;
            }

            InvalidateMeasure();
        }
    }
}