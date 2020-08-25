using AnnotationsLib.Annotations;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AnnotationsLib
{
    /// <summary>
    /// Logique d'interaction pour AnnotationsControl.xaml
    /// </summary>
    public partial class AnnotationsControl : Viewbox
    {
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(AnnotationsControl), new PropertyMetadata(false));
        public static readonly DependencyProperty ContainerWidthProperty = DependencyProperty.Register("ContainerWidth", typeof(double), typeof(AnnotationsControl));
        public static readonly DependencyProperty ContainerHeightProperty = DependencyProperty.Register("ContainerHeight", typeof(double), typeof(AnnotationsControl));
        public static readonly DependencyProperty AnnotationsProperty = DependencyProperty.Register("Annotations", typeof(ObservableCollection<UIElement>), typeof(AnnotationsControl), new PropertyMetadata(new ObservableCollection<UIElement>()));
        public static readonly DependencyProperty SelectedAnnotationTypeProperty = DependencyProperty.Register("SelectedAnnotationType", typeof(Type), typeof(AnnotationsControl));
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(AnnotationsControl));

        public FrameworkElement AdorneredElement { get; private set; }

        public bool IsInEditMode
        {
            get { return (bool)GetValue(IsInEditModeProperty); }
            set
            {
                SetValue(IsInEditModeProperty, value);
                if (value)
                {
                    annotationsContainer.MouseLeftButtonDown += annotationsContainer_MouseLeftButtonDown;
                    annotationsContainer.MouseLeftButtonUp += AnnotationsContainer_MouseLeftButtonUp;
                }
                else
                {
                    annotationsContainer.MouseLeftButtonDown -= annotationsContainer_MouseLeftButtonDown;
                    annotationsContainer.MouseLeftButtonUp -= AnnotationsContainer_MouseLeftButtonUp;
                }
            }
        }

        public double ContainerWidth
        {
            get { return (double)GetValue(ContainerWidthProperty); }
            set { SetValue(ContainerWidthProperty, value); }
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(ContainerHeightProperty); }
            set { SetValue(ContainerHeightProperty, value); }
        }

        public ObservableCollection<UIElement> Annotations
        {
            get { return (ObservableCollection<UIElement>)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        public Type SelectedAnnotationType
        {
            get { return (Type)GetValue(SelectedAnnotationTypeProperty); }
            set { SetValue(SelectedAnnotationTypeProperty, value); }
        }

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        public void AddAnnotation(AnnotationBase annotation)
        {
            annotation.SetBinding(AnnotationBase.IsInEditModeProperty, new Binding(IsInEditModeProperty.Name) { Source = this });
            Annotations.Add(annotation);
        }

        public void RemoveAnnotation(AnnotationBase annotation) => Annotations.Remove(annotation);

        public void ClearAnnotations() => Annotations.Clear();

        public void CleanUp()
        {
            Annotations.CollectionChanged -= Annotations_CollectionChanged;
            //this.Loaded -= forTest;
        }

        private void SetCommands()
        {
            AnnotationsAdornment.SetSetAnnotationTypeCommand(AdorneredElement, new DelegateCommand<Type>(e => SelectedAnnotationType = e, e => e == null ? false : e.IsSubclassOf(typeof(AnnotationBase))));
            AnnotationsAdornment.SetSetBrushCommand(AdorneredElement, new DelegateCommand<Brush>(e => SelectedBrush = e));
            AnnotationsAdornment.SetClearAnnotationsCommand(AdorneredElement, new DelegateCommand(e => ClearAnnotations()));
            AnnotationsAdornment.SetSaveCommand(AdorneredElement, new DelegateCommand<Stream>(e =>
            {
                var oldEditValue = IsInEditMode;
                IsInEditMode = false;
                JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder { QualityLevel = 90 };
                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)ContainerWidth, (int)ContainerHeight, 96, 96, PixelFormats.Pbgra32);

                VisualBrush sourceBrush = new VisualBrush(AdorneredElement);
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                using (drawingContext)
                {
                    drawingContext.PushTransform((Transform)AdorneredElement.LayoutTransform.Inverse);
                    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(ContainerWidth, ContainerHeight)));
                }
                renderTarget.Render(drawingVisual);
                jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

                sourceBrush = new VisualBrush(annotationsContainer);
                drawingVisual = new DrawingVisual();
                drawingContext = drawingVisual.RenderOpen();
                using (drawingContext)
                {
                    drawingContext.PushTransform((Transform)annotationsContainer.LayoutTransform.Inverse);
                    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(ContainerWidth, ContainerHeight)));
                }
                renderTarget.Render(drawingVisual);
                jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
                jpgEncoder.Save(e);
                IsInEditMode = oldEditValue;
            }, e => e != null));
        }

        public AnnotationsControl()
        {
            InitializeComponent();
            Annotations.CollectionChanged += Annotations_CollectionChanged;

            //this.Loaded += forTest;
        }

        public AnnotationsControl(FrameworkElement adorneredElement):this()
        {
            SetBinding(WidthProperty, new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = adorneredElement });
            SetBinding(HeightProperty, new Binding(FrameworkElement.ActualHeightProperty.Name) { Source = adorneredElement });
            AdorneredElement = adorneredElement;
            SetCommands();
        }

        /*private void forTest(object sender, RoutedEventArgs e)
        {
            AddAnnotation(new TextAnnotation(20, 20, 400, 100, "TEST_INK\nessai avec un texte plus long", Brushes.Yellow));
            AddAnnotation(new RectangleAnnotation(500, 80, 100, 60, Brushes.Red, 3));
            AddAnnotation(new ArrowAnnotation(20, 200, 300, 220, Brushes.Red, 6));
            AddAnnotation(new LineAnnotation(500, 200, 600, 300, Brushes.Green, 6));
            AddAnnotation(new LineAnnotation(600, 200, 500, 300, Brushes.Green, 6));
            AddAnnotation(new MagnifierAnnotation(500, 300, Brushes.Blue) { Radius = 60, Thickness = 2 });
        }*/

        private void Annotations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    e.NewItems.OfType<AnnotationBase>().ToList().ForEach(_ => annotationsContainer.Children.Add(_));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems.OfType<AnnotationBase>().ToList().ForEach(_ => annotationsContainer.Children.Remove(_));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    var toRemove = annotationsContainer.Children.OfType<AnnotationBase>();
                    toRemove.ToList().ForEach(_ => annotationsContainer.Children.Remove(_));
                    break;
                default:
                    break;
            }
        }

        private bool _cancelAddAnnotation = false;

        private async void annotationsContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _lastPosition = e.GetPosition(sender as IInputElement);
            await Task.Delay(100);

            if (!_cancelAddAnnotation)
            {
                var _lastCreateAnnotation = AnnotationBase.Create(SelectedAnnotationType, _lastPosition.X, _lastPosition.Y, SelectedBrush);
                AddAnnotation(_lastCreateAnnotation);
                var focusedElement = await _lastCreateAnnotation.GetElementToFocusOnCreating();
                if (focusedElement != null)
                {
                    focusedElement.CaptureMouse();
                    focusedElement.RaiseEvent(e);
                }
            }

            _cancelAddAnnotation = false;
            e.Handled = true;
        }

        private void AnnotationsContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _cancelAddAnnotation = true;
        }
    }
}
