using AnnotationsLib.Annotations.Actions;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AnnotationsLib.Annotations
{
    public abstract class AnnotationBase : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(AnnotationBase), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(double), typeof(AnnotationBase));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(double), typeof(AnnotationBase));
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(AnnotationBase));
        public static readonly DependencyProperty ActionsAdornerProperty = DependencyProperty.Register("ActionsAdorner", typeof(ActionsAnnotationAdorner), typeof(AnnotationBase));
        public static readonly DependencyProperty ResizingAdornerProperty = DependencyProperty.Register("ResizingAdorner", typeof(ResizingAdorner), typeof(AnnotationBase));

        protected bool isRefreshing;

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyChanged(DependencyProperty dp)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dp.Name));
        }

        public bool IsInEditMode
        {
            get { return (bool)GetValue(IsInEditModeProperty); }
            set { SetValue(IsInEditModeProperty, value); NotifyChanged(IsInEditModeProperty); }
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

        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public ActionsAnnotationAdorner ActionsAdorner
        {
            get { return (ActionsAnnotationAdorner)GetValue(ActionsAdornerProperty); }
            set { SetValue(ActionsAdornerProperty, value); }
        }

        public ResizingAdorner ResizingAdorner
        {
            get { return (ResizingAdorner)GetValue(ResizingAdornerProperty); }
            set { SetValue(ResizingAdornerProperty, value); }
        }

        public IMultiValueConverter ActionMenuPositionConverter { get; set; } = new DefaultActionsMenuPositionConverter();

        public BindingList<AnnotationAction> Actions { get; private set; }

        static AnnotationBase _lastAnnotationMouseOver;
        Timer VisibilityTimer;

        protected void Edit_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_lastAnnotationMouseOver != null && _lastAnnotationMouseOver != this)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    if (_lastAnnotationMouseOver.ActionsAdorner != null)
                        _lastAnnotationMouseOver.ActionsAdorner.Visibility = Visibility.Hidden;
                });
                _lastAnnotationMouseOver.VisibilityTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _lastAnnotationMouseOver.VisibilityTimer = null;
                _lastAnnotationMouseOver = null;
            }
            if (VisibilityTimer != null)
            {
                VisibilityTimer.Change(Timeout.Infinite, Timeout.Infinite);
                VisibilityTimer = null;
            }
            if (ActionsAdorner.Visibility == Visibility.Hidden)
            {
                _lastAnnotationMouseOver = this;
                ActionsAdorner.Opacity = 0.0;
                ActionsAdorner.Visibility = Visibility.Visible;
                ActionsAdorner.BeginAnimation(OpacityProperty, FadeIn);
            }
        }

        protected void Edit_MouseLeave(object sender, MouseEventArgs e)
        {
            VisibilityTimer = new Timer(VisibilityTimerCallback, null, 1000, Timeout.Infinite);
        }

        static readonly DoubleAnimation FadeIn = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(500)), FillBehavior.HoldEnd);
        static readonly DoubleAnimation FadeOut = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(500)), FillBehavior.HoldEnd);

        void VisibilityTimerCallback(object state)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                if (ActionsAdorner != null)
                {
                    ActionsAdorner.Opacity = 1.0;
                    ActionsAdorner.BeginAnimation(OpacityProperty, FadeOut);
                    ActionsAdorner.Visibility = Visibility.Hidden;
                }
            });
            VisibilityTimer.Change(Timeout.Infinite, Timeout.Infinite);
            VisibilityTimer = null;
            _lastAnnotationMouseOver = null;
        }

        public virtual Task<FrameworkElement> GetElementToFocusOnCreating() => null;

        public static AnnotationBase Create(Type type, double x, double y, Brush brush)
        {
            if (type.IsSubclassOf(typeof(AnnotationBase)))
                return (AnnotationBase)Activator.CreateInstance(type, x, y, brush);
            return null;
        }

        protected AnnotationBase()
        {
            Actions = new BindingList<AnnotationAction>
            {
                new ChangeBrushAction(this),
                new DeleteAction(this)
            };
            ActionsAdorner = new ActionsAnnotationAdorner { ItemsSource = Actions, DataContext = this };
            ActionsAdorner.MouseEnter += Edit_MouseEnter;
            ActionsAdorner.MouseLeave += Edit_MouseLeave;
        }

        class DefaultActionsMenuPositionConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values != null && values.Length >= 6
                    && values[0] is AnnotationBase annotation
                    && values[1] is double inkCanvasPosition
                    && values[2] is double resizingSize
                    && values[3] is double originalSize
                    && values[4] is double size
                    && values[5] is double menuSize)
                {
                    string param = parameter as string;

                    double ratio = resizingSize / originalSize;

                    if (param == "Left")
                        return (size / 2 + inkCanvasPosition) * ratio - (menuSize / 2);
                    if (param == "Top")
                        return inkCanvasPosition * ratio - menuSize - (annotation.ResizingAdorner == null ? 0 : annotation.ResizingAdorner.HandleSize);
                    return inkCanvasPosition * ratio;
                }
                return double.NaN;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
