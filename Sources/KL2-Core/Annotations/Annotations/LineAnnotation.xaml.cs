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
    /// Logique d'interaction pour LineAnnotation.xaml
    /// </summary>
    public partial class LineAnnotation : AnnotationBase, IThicknessEditableAnnotation
    {
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(LineAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(LineAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(LineAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(LineAnnotation), new FrameworkPropertyMetadata(0.0d, UpdateCoordinates));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(LineAnnotation), new FrameworkPropertyMetadata(3.0, OnThicknessPropertyChanged));

        private static readonly Dictionary<DependencyProperty, (DependencyProperty Property, DependencyProperty SimilarProperty)> SimilarsProperties = new Dictionary<DependencyProperty, (DependencyProperty Property, DependencyProperty SimilarProperty)>()
        {
            [X1Property] = (X2Property, XProperty),
            [Y1Property] = (Y2Property, YProperty),
            [X2Property] = (X1Property, XProperty),
            [Y2Property] = (Y1Property, YProperty)
        };

        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (LineAnnotation)d;

            var increaseCommand = annotation.Actions.OfType<IncreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            var decreaseCommand = annotation.Actions.OfType<DecreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            increaseCommand?.RaiseCanExecuteChanged();
            decreaseCommand?.RaiseCanExecuteChanged();
        }

        private static void OnIsInEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => OnThicknessPropertyChanged(d, e);

        private static void UpdateCoordinates(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineAnnotation obj = (LineAnnotation)d;
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
            while(resizingAdorner == null)
            {
                await Task.Delay(10);
                resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            }
            return resizingAdorner.PART_THUMB_POINT2;
        }

        static LineAnnotation()
        {
            IsInEditModeProperty.OverrideMetadata(typeof(LineAnnotation), new FrameworkPropertyMetadata(OnIsInEditModePropertyChanged));
        }

        public LineAnnotation()
        {
            InitializeComponent();
            Actions.Insert(0, new IncreaseThicknessAction(this));
            Actions.Insert(0, new DecreaseThicknessAction(this));
        }

        public LineAnnotation(double x1, double y1, double? x2, double? y2, Brush brush, double? thickness) : this()
        {
            X = Math.Min(x1, x2 ?? x1);
            Y = Math.Min(y1, y2 ?? y1);

            isRefreshing = true;
            X1 = x1 - X;
            Y1 = y1 - Y;
            X2 = x2 == null ? X1 : x2.Value - X;
            Y2 = y2 == null ? Y1 : y2.Value - Y;
            isRefreshing = false;

            Brush = brush;
            if (thickness != null) Thickness = thickness.Value;
        }

        public LineAnnotation(double x, double y, Brush brush) : this()
        {
            X = x;
            Y = y;
            X1 = 0;
            Y1 = 0;
            X2 = 0;
            Y2 = 0;
            Brush = brush;
        }
    }
}
