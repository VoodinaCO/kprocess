using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnnotationsLib.Controls
{
    public class Arrow : Shape
    {
        public static readonly DependencyProperty StartXProperty = DependencyProperty.Register("StartX", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, RefreshMeasure));
        public static readonly DependencyProperty StartYProperty = DependencyProperty.Register("StartY", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, RefreshMeasure));
        public static readonly DependencyProperty EndXProperty = DependencyProperty.Register("EndX", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, RefreshMeasure));
        public static readonly DependencyProperty EndYProperty = DependencyProperty.Register("EndY", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, RefreshMeasure));

        public double StartX
        {
            get { return (double)GetValue(StartXProperty); }
            set { SetValue(StartXProperty, value); }
        }

        public double StartY
        {
            get { return (double)GetValue(StartYProperty); }
            set { SetValue(StartYProperty, value); }
        }

        public double EndX
        {
            get { return (double)GetValue(EndXProperty); }
            set { SetValue(EndXProperty, value); }
        }

        public double EndY
        {
            get { return (double)GetValue(EndYProperty); }
            set { SetValue(EndYProperty, value); }
        }

        private static void RefreshMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)d).InvalidateMeasure();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                LineGeometry lineGeometry = new LineGeometry(new Point(StartX,StartY),new Point(EndX,EndY));
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double theta = Math.Atan2(StartY - EndY, StartX - EndX);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            //Line
            Point pt1 = new Point(StartX, StartY);
            Point pt2 = new Point(EndX, EndY);

            //Arrow
            Point pt3 = new Point(
                EndX + (cost - sint) * StrokeThickness * 2,
                EndY + (sint + cost) * StrokeThickness * 2);

            Point pt4 = new Point(
                EndX + (cost + sint) * StrokeThickness * 2,
                EndY - (cost - sint) * StrokeThickness * 2);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt4, true, true);
            context.LineTo(pt2, true, true);
        }

        public Arrow():base()
        { }
    }
}
