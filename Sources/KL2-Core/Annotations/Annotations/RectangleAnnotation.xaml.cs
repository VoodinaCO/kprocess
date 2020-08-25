using AnnotationsLib.Annotations.Actions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnnotationsLib.Annotations
{
    /// <summary>
    /// Logique d'interaction pour RectangleAnnotation.xaml
    /// </summary>
    public partial class RectangleAnnotation : AnnotationBase, IThicknessEditableAnnotation
    {
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(RectangleAnnotation), new FrameworkPropertyMetadata(3.0, OnThicknessPropertyChanged));
        
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (RectangleAnnotation)d;

            var increaseCommand = annotation.Actions.OfType<IncreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            var decreaseCommand = annotation.Actions.OfType<DecreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            increaseCommand?.RaiseCanExecuteChanged();
            decreaseCommand?.RaiseCanExecuteChanged();
        }

        private static void OnIsInEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => OnThicknessPropertyChanged(d, e);

        public override async Task<FrameworkElement> GetElementToFocusOnCreating()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            var resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            while (resizingAdorner == null)
            {
                await Task.Delay(10);
                resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            }
            return resizingAdorner.PART_THUMB_SE;
        }

        static RectangleAnnotation()
        {
            IsInEditModeProperty.OverrideMetadata(typeof(RectangleAnnotation), new FrameworkPropertyMetadata(OnIsInEditModePropertyChanged));
        }

        public RectangleAnnotation()
        {
            InitializeComponent();
            Actions.Insert(0, new IncreaseThicknessAction(this));
            Actions.Insert(0, new DecreaseThicknessAction(this));
        }

        public RectangleAnnotation(double x, double y, double? width, double? height, Brush brush, double? thickness) : this()
        {
            X = x;
            Y = y;
            Width = width == null ? 0 : width.Value;
            Height = height == null ? 0 : height.Value;
            Brush = brush;
            if (thickness != null) Thickness = thickness.Value;
        }

        public RectangleAnnotation(double x, double y, Brush brush) : this()
        {
            X = x;
            Y = y;
            Width = 0;
            Height = 0;
            Brush = brush;
        }
    }
}
