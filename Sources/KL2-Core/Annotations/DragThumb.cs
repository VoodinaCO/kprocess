using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AnnotationsLib
{
    public class DragThumb : Thumb
    {


        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));



        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));



        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("MinX", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d));



        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register("MinY", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d));



        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register("MaxX", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d));



        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register("MaxY", typeof(double), typeof(DragThumb), new FrameworkPropertyMetadata(0.0d));



        public DragThumb()
        {
            this.DragDelta += DragThumb_DragDelta;
        }

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            X = Math.Min(MaxX, Math.Max(MinX, X + e.HorizontalChange));
            Y = Math.Min(MaxY, Math.Max(MinY, Y + e.VerticalChange));
        }
    }
}
