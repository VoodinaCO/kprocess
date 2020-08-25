using AnnotationsLib.Annotations.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnnotationsLib.Annotations
{
    /// <summary>
    /// Logique d'interaction pour DoubleArrowAnnotation.xaml
    /// </summary>
    public partial class DoubleArrowAnnotation : AnnotationBase, IThicknessEditableAnnotation
    {
        public static readonly DependencyProperty StartXProperty = DependencyProperty.Register("StartX", typeof(double), typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty StartYProperty = DependencyProperty.Register("StartY", typeof(double), typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty EndXProperty = DependencyProperty.Register("EndX", typeof(double), typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty EndYProperty = DependencyProperty.Register("EndY", typeof(double), typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(3.0, OnThicknessPropertyChanged));

        private static readonly Dictionary<DependencyProperty, (DependencyProperty Property, DependencyProperty SimilarProperty)> SimilarsProperties = new Dictionary<DependencyProperty, (DependencyProperty Property, DependencyProperty SimilarProperty)>()
        {
            [StartXProperty] = (EndXProperty, XProperty),
            [StartYProperty] = (EndYProperty, YProperty),
            [EndXProperty] = (StartXProperty, XProperty),
            [EndYProperty] = (StartYProperty, YProperty)
        };

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

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (DoubleArrowAnnotation)d;

            var increaseCommand = annotation.Actions.OfType<IncreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            var decreaseCommand = annotation.Actions.OfType<DecreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            increaseCommand?.RaiseCanExecuteChanged();
            decreaseCommand?.RaiseCanExecuteChanged();
        }

        private static void OnIsInEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => OnThicknessPropertyChanged(d, e);

        private static void UpdateCoordinates(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleArrowAnnotation obj = (DoubleArrowAnnotation)d;
            DependencyProperty changedProperty = e.Property;
            DependencyProperty similarProperty = SimilarsProperties[changedProperty].Property;
            DependencyProperty coordProperty = SimilarsProperties[changedProperty].SimilarProperty;
            obj.isRefreshing = true;
            if ((double)e.NewValue < 0 || (double)e.NewValue < (double)d.GetValue(similarProperty))
            {
                d.SetValue(changedProperty, 0.0d);
                d.SetValue(similarProperty, (double)d.GetValue(similarProperty) - (double)e.NewValue);
                d.SetValue(coordProperty, (double)d.GetValue(coordProperty) + (double)e.NewValue);
            }
            obj.isRefreshing = false;
        }

        public override async Task<FrameworkElement> GetElementToFocusOnCreating()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            var resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            while (resizingAdorner == null)
            {
                await Task.Delay(10);
                resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            }
            return resizingAdorner.PART_THUMB_POINT2;
        }

        static DoubleArrowAnnotation()
        {
            IsInEditModeProperty.OverrideMetadata(typeof(DoubleArrowAnnotation), new FrameworkPropertyMetadata(OnIsInEditModePropertyChanged));
        }

        public DoubleArrowAnnotation()
        {
            InitializeComponent();
            Actions.Insert(0, new IncreaseThicknessAction(this));
            Actions.Insert(0, new DecreaseThicknessAction(this));
        }

        public DoubleArrowAnnotation(double startX, double startY, double? endX, double? endY, Brush brush, double? thickness) : this()
        {
            X = Math.Min(startX, endX ?? startX);
            Y = Math.Min(startY, endY ?? startY);

            isRefreshing = true;
            StartX = startX - X;
            StartY = startY - Y;
            EndX = endX == null ? StartX : endX.Value - X;
            EndY = endY == null ? StartY : endY.Value - Y;
            isRefreshing = false;

            Brush = brush;
            if (thickness != null) Thickness = thickness.Value;
        }

        public DoubleArrowAnnotation(double x, double y, Brush brush) : this()
        {
            X = x;
            Y = y;
            StartX = 0;
            StartY = 0;
            EndX = 0;
            EndY = 0;
            Brush = brush;
        }
    }
}
