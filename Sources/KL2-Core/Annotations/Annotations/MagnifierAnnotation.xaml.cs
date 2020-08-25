using AnnotationsLib.Annotations.Actions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnnotationsLib.Annotations
{
    /// <summary>
    /// Logique d'interaction pour MagnifierAnnotation.xaml
    /// </summary>

    public partial class MagnifierAnnotation : AnnotationBase, IThicknessEditableAnnotation, IZoomEditableAnnotation
    {
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(MagnifierAnnotation), new FrameworkPropertyMetadata(3d, OnThicknessPropertyChanged));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(MagnifierAnnotation), new FrameworkPropertyMetadata(0d, OnRadiusPropertyChanged));
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(MagnifierAnnotation), new FrameworkPropertyMetadata(2d, OnZoomFactorPropertyChanged));

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (MagnifierAnnotation)d;

            var increaseCommand = annotation.Actions.OfType<IncreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            var decreaseCommand = annotation.Actions.OfType<DecreaseThicknessAction>().DefaultIfEmpty(null).FirstOrDefault();
            increaseCommand?.RaiseCanExecuteChanged();
            decreaseCommand?.RaiseCanExecuteChanged();
        }

        private static void OnZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (MagnifierAnnotation)d;

            var increaseCommand = annotation.Actions.OfType<IncreaseZoomAction>().DefaultIfEmpty(null).FirstOrDefault();
            var decreaseCommand = annotation.Actions.OfType<DecreaseZoomAction>().DefaultIfEmpty(null).FirstOrDefault();
            increaseCommand?.RaiseCanExecuteChanged();
            decreaseCommand?.RaiseCanExecuteChanged();
        }

        private static void OnIsInEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnThicknessPropertyChanged(d, e);
            OnZoomFactorPropertyChanged(d, e);
        }

        private static void OnRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MagnifierAnnotation control = d as MagnifierAnnotation;
            var offset = (double)e.NewValue - (double)e.OldValue;
            control.Width = control.Radius * 2;
            control.Height = control.Radius * 2;
            control.X -= offset;
            control.Y -= offset;
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
            return resizingAdorner.PART_THUMB_S;
        }

        static MagnifierAnnotation()
        {
            IsInEditModeProperty.OverrideMetadata(typeof(MagnifierAnnotation), new FrameworkPropertyMetadata(OnIsInEditModePropertyChanged));
        }

        public MagnifierAnnotation()
        {
            InitializeComponent();
            Actions.Insert(0, new IncreaseThicknessAction(this));
            Actions.Insert(0, new DecreaseThicknessAction(this));
            Actions.Insert(0, new IncreaseZoomAction(this));
            Actions.Insert(0, new DecreaseZoomAction(this));
        }

        public MagnifierAnnotation(double x, double y, Brush brush) : this()
        {
            X = x;
            Y = y;
            Brush = brush;
            Width = 0.1d;
            Height = 0.1d;
        }
    }
}
