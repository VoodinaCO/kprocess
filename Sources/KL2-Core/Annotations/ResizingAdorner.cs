using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AnnotationsLib
{
    [TemplatePart(Name = _PART_THUMB_MOVE, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_NW, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_NE, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_SW, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_NW, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_SE, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_N, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_E, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_S, Type = typeof(Thumb))]
    [TemplatePart(Name = _PART_THUMB_W, Type = typeof(Thumb))]
    public class ResizingAdorner : ContentControl
    {
        private const string _PART_THUMB_MOVE = "PART_THUMB_MOVE";
        private const string _PART_THUMB_NW = "PART_THUMB_NW";
        private const string _PART_THUMB_NE = "PART_THUMB_NE";
        private const string _PART_THUMB_SW = "PART_THUMB_SW";
        private const string _PART_THUMB_SE = "PART_THUMB_SE";
        private const string _PART_THUMB_N = "PART_THUMB_N";
        private const string _PART_THUMB_E = "PART_THUMB_E";
        private const string _PART_THUMB_S = "PART_THUMB_S";
        private const string _PART_THUMB_W = "PART_THUMB_W";
        private const string _PART_THUMB_POINT1 = "PART_THUMB_POINT1";
        private const string _PART_THUMB_POINT2 = "PART_THUMB_POINT2";

        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SingleProperty = DependencyProperty.Register("Single", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MinXProperty = DependencyProperty.Register("MinX", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0.0d));
        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register("MaxX", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0.0d));
        public static readonly DependencyProperty MinYProperty = DependencyProperty.Register("MinY", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0.0d));
        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register("MaxY", typeof(double), typeof(ResizingAdorner), new FrameworkPropertyMetadata(0.0d));
        public static readonly DependencyProperty HandleSizeProperty = DependencyProperty.Register("HandleSize", typeof(double), typeof(ResizingAdorner), new PropertyMetadata(3d));

        public Thumb PART_THUMB_MOVE { get; private set; }
        public Thumb PART_THUMB_NW { get; private set; }
        public Thumb PART_THUMB_NE { get; private set; }
        public Thumb PART_THUMB_SW { get; private set; }
        public Thumb PART_THUMB_SE { get; private set; }
        public Thumb PART_THUMB_N { get; private set; }
        public Thumb PART_THUMB_E { get; private set; }
        public Thumb PART_THUMB_S { get; private set; }
        public Thumb PART_THUMB_W { get; private set; }
        public DragThumb PART_THUMB_POINT1 { get; private set; }
        public DragThumb PART_THUMB_POINT2 { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_THUMB_MOVE = GetTemplateChild(_PART_THUMB_MOVE) as Thumb;
            PART_THUMB_NW = GetTemplateChild(_PART_THUMB_NW) as Thumb;
            PART_THUMB_NE = GetTemplateChild(_PART_THUMB_NE) as Thumb;
            PART_THUMB_SW = GetTemplateChild(_PART_THUMB_SW) as Thumb;
            PART_THUMB_SE = GetTemplateChild(_PART_THUMB_SE) as Thumb;
            PART_THUMB_N = GetTemplateChild(_PART_THUMB_N) as Thumb;
            PART_THUMB_E = GetTemplateChild(_PART_THUMB_E) as Thumb;
            PART_THUMB_S = GetTemplateChild(_PART_THUMB_S) as Thumb;
            PART_THUMB_W = GetTemplateChild(_PART_THUMB_W) as Thumb;
            PART_THUMB_POINT1 = GetTemplateChild(_PART_THUMB_POINT1) as DragThumb;
            PART_THUMB_POINT2 = GetTemplateChild(_PART_THUMB_POINT2) as DragThumb;
        }

        public ResizingAdorner()
        {
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDeltaEvent));
        }

        static ResizingAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizingAdorner), new FrameworkPropertyMetadata(typeof(ResizingAdorner)));
            WidthProperty.OverrideMetadata(typeof(ResizingAdorner), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
            HeightProperty.OverrideMetadata(typeof(ResizingAdorner), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        }

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public double Single
        {
            get { return (double)GetValue(SingleProperty); }
            set { SetValue(SingleProperty, value); }
        }

        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        public double HandleSize
        {
            get { return (double)GetValue(HandleSizeProperty); }
            set { SetValue(HandleSizeProperty, value); }
        }

        private void OnThumbDragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            string thumbName = ((Thumb)e.Source).Name;
            if (thumbName == _PART_THUMB_MOVE)
            {
                X = Math.Min(MaxX, Math.Max(MinX, X + e.HorizontalChange));
                Y = Math.Min(MaxY, Math.Max(MinY, Y + e.VerticalChange));
            }
            if (thumbName == _PART_THUMB_NW || thumbName == _PART_THUMB_SW) // Adjust left
            {
                double oldWidth = Width;
                var xChange = X + e.HorizontalChange < MinX ? -X : e.HorizontalChange;
                Width = Math.Min(Math.Max(Width - xChange, MinWidth), MaxWidth);
                X += oldWidth - Width;
            }
            if (thumbName == _PART_THUMB_NW || thumbName == _PART_THUMB_NE) // Adjust top
            {
                double oldHeight = Height;
                var yChange = Y + e.VerticalChange < MinY ? -Y : e.VerticalChange;
                Height = Math.Min(Math.Max(Height - yChange, MinHeight), MaxHeight);
                Y += oldHeight - Height;
            }
            if (thumbName == _PART_THUMB_NE || thumbName == _PART_THUMB_SE) // Adjust right
            {
                Width = Math.Min(Math.Max(Width + e.HorizontalChange, MinWidth), MaxWidth);
            }
            if (thumbName == _PART_THUMB_SW || thumbName == _PART_THUMB_SE) // Adjust bottom
            {
                Height = Math.Min(Math.Max(Height + e.VerticalChange, MinHeight), MaxHeight);
            }
            if (thumbName == _PART_THUMB_N)
            {
                var deltas = new double[] { X, Y, MaxWidth - Width, MaxHeight - Height };
                var maxSingle = Single + deltas.Min();
                Single = Math.Min(Math.Max(Single - e.VerticalChange, MinY), maxSingle);
            }
            if (thumbName == _PART_THUMB_S)
            {
                var deltas = new double[] { X, Y, MaxWidth - Width, MaxHeight - Height };
                var maxSingle = Single + deltas.Min();
                Single = Math.Min(Math.Max(Single + e.VerticalChange, MinY), maxSingle);
            }
            if (thumbName == _PART_THUMB_W)
            {
                var deltas = new double[] { X, Y, MaxWidth - Width, MaxHeight - Height };
                var maxSingle = Single + deltas.Min();
                Single = Math.Min(Math.Max(Single - e.HorizontalChange, MinX), maxSingle);
           }
            if (thumbName == _PART_THUMB_E)
            {
                var deltas = new double[] { X, Y, MaxWidth - Width, MaxHeight - Height };
                var maxSingle = Single + deltas.Min();
                Single = Math.Min(Math.Max(Single + e.HorizontalChange, MinX), maxSingle);
            }
        }
    }
}
