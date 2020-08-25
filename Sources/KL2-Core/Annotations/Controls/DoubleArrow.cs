using System;
using System.Windows;
using System.Windows.Media;

namespace AnnotationsLib.Controls
{
    public class DoubleArrow : Arrow
    {
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

            //Start Arrow
            Point pt2 = new Point(
                StartX - (cost - sint) * StrokeThickness * 2,
                StartY - (sint + cost) * StrokeThickness * 2);
            Point pt3 = new Point(
                StartX - (cost + sint) * StrokeThickness * 2,
                StartY + (cost - sint) * StrokeThickness * 2);

            //Line
            Point pt1 = new Point(StartX, StartY);
            Point pt4 = new Point(EndX, EndY);

            //End Arrow
            Point pt5 = new Point(
                EndX + (cost - sint) * StrokeThickness * 2,
                EndY + (sint + cost) * StrokeThickness * 2);
            Point pt6 = new Point(
                EndX + (cost + sint) * StrokeThickness * 2,
                EndY - (cost - sint) * StrokeThickness * 2);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt1, true, true);
            context.LineTo(pt4, true, true);
            context.LineTo(pt5, true, true);
            context.LineTo(pt6, true, true);
            context.LineTo(pt4, true, true);
        }

        public DoubleArrow():base()
        { }
    }
}
