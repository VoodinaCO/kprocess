using AnnotationsLib.Annotations;
using AnnotationsLib.Converters;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AnnotationsLib
{
    public class AnnotationsAdornment : Adornment
    {
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.RegisterAttached("IsInEditMode", typeof(bool), typeof(AnnotationsAdornment), new PropertyMetadata(false, IsInEditModeChanged));
        public static readonly DependencyProperty AnnotationsIsVisibleProperty = DependencyProperty.RegisterAttached("AnnotationsIsVisible", typeof(bool), typeof(AnnotationsAdornment), new PropertyMetadata(false, AnnotationsIsVisibleChanged));

        public static readonly DependencyProperty SetAnnotationTypeCommandProperty = DependencyProperty.RegisterAttached("SetAnnotationTypeCommand", typeof(ICommand), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty SetBrushCommandProperty = DependencyProperty.RegisterAttached("SetBrushCommand", typeof(ICommand), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.RegisterAttached("SaveCommand", typeof(ICommand), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty ClearAnnotationsCommandProperty = DependencyProperty.RegisterAttached("ClearAnnotationsCommand", typeof(ICommand), typeof(AnnotationsAdornment));

        public static readonly DependencyProperty AnnotationsLayerProperty = DependencyProperty.RegisterAttached("AnnotationsLayer", typeof(AnnotationsControl), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty ActionsLayerProperty = DependencyProperty.RegisterAttached("ActionsLayer", typeof(CanvasLayer), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty MenuLayerProperty = DependencyProperty.RegisterAttached("MenuLayer", typeof(CanvasLayer), typeof(AnnotationsAdornment));
        public static readonly DependencyProperty AnnotationsChangedDelegateProperty = DependencyProperty.RegisterAttached("AnnotationsChangedDelegate", typeof(NotifyCollectionChangedEventHandler), typeof(AnnotationsAdornment));

        public static void SetAnnotationsLayer(DependencyObject element, AnnotationsControl value) => element.SetValue(AnnotationsLayerProperty, value);
        public static AnnotationsControl GetAnnotationsLayer(DependencyObject element) => (AnnotationsControl)element.GetValue(AnnotationsLayerProperty);

        public static void SetActionsLayer(DependencyObject element, CanvasLayer value) => element.SetValue(ActionsLayerProperty, value);
        public static CanvasLayer GetActionsLayer(DependencyObject element) => (CanvasLayer)element.GetValue(ActionsLayerProperty);

        public static void SetMenuLayer(DependencyObject element, CanvasLayer value) => element.SetValue(MenuLayerProperty, value);
        public static CanvasLayer GetMenuLayer(DependencyObject element) => (CanvasLayer)element.GetValue(MenuLayerProperty);

        public static void SetAnnotationsChangedDelegate(DependencyObject element, NotifyCollectionChangedEventHandler value) => element.SetValue(AnnotationsChangedDelegateProperty, value);
        public static NotifyCollectionChangedEventHandler GetAnnotationsChangedDelegate(DependencyObject element) => (NotifyCollectionChangedEventHandler)element.GetValue(AnnotationsChangedDelegateProperty);

        public static void SetSetAnnotationTypeCommand(DependencyObject element, ICommand value) => element.SetValue(SetAnnotationTypeCommandProperty, value);
        public static ICommand GetSetAnnotationTypeCommandProperty(DependencyObject element) => (ICommand)element.GetValue(SetAnnotationTypeCommandProperty);

        public static void SetSetBrushCommand(DependencyObject element, ICommand value) => element.SetValue(SetBrushCommandProperty, value);
        public static ICommand GetSetBrushCommand(DependencyObject element) => (ICommand)element.GetValue(SetBrushCommandProperty);

        public static void SetSaveCommand(DependencyObject element, ICommand value) => element.SetValue(SaveCommandProperty, value);
        public static ICommand GetSaveCommand(DependencyObject element) => (ICommand)element.GetValue(SaveCommandProperty);

        public static void SetClearAnnotationsCommand(DependencyObject element, ICommand value) => element.SetValue(ClearAnnotationsCommandProperty, value);
        public static ICommand GetClearAnnotationsCommand(DependencyObject element) => (ICommand)element.GetValue(ClearAnnotationsCommandProperty);

        /// <summary>
        /// Gets/Sets the edit mode of this element's adorner layer
        /// </summary>
        public static bool GetIsInEditMode(DependencyObject element) => (bool)element.GetValue(IsInEditModeProperty);
        public static void SetIsInEditMode(DependencyObject element, bool value) => element.SetValue(IsInEditModeProperty, value);

        /// <summary>
        /// Gets/Sets the visibility of this element's adorner layer
        /// </summary>
        public static bool GetAnnotationsIsVisible(DependencyObject element) => (bool)element.GetValue(AnnotationsIsVisibleProperty);
        public static void SetAnnotationsIsVisible(DependencyObject element, bool value) => element.SetValue(AnnotationsIsVisibleProperty, value);

        protected static async void IsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            while (GetContent(d) == null)
            {
                await Task.Delay(100);
            }
            var layer = AdornerLayer.GetAdornerLayer(d as Visual);
            var adorners = layer.GetAdorners(d as UIElement).OfType<CanvasAdorner>();
            var control = adorners.SelectMany(_ => _.Elements.OfType<AnnotationsControl>()).First();
            control.IsInEditMode = (bool)e.NewValue;
        }

        protected static void AnnotationsIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && GetContent(d) == null)
            {
                var image = d as Image;
                var uielement = d as FrameworkElement;
                var width = image?.Source?.Width ?? uielement.Width;
                var height = image?.Source?.Height ?? uielement.Height;
                AdornmentCollection layers = new AdornmentCollection();

                SetAnnotationsLayer(d, new AnnotationsControl((FrameworkElement)d) { Name = "AnnotationsLayer", ContainerWidth = width, ContainerHeight = height });
                SetActionsLayer(d, CanvasLayer.New<ActionsAnnotationAdorner>((FrameworkElement)d, "ActionsLayer"));
                SetMenuLayer(d, CanvasLayer.New<UIElement>((FrameworkElement)d, "MenuLayer"));
                GetMenuLayer(d).AnnotationsControl = GetAnnotationsLayer(d);
                var controlMenu = new AnnotationsControlMenu();
                controlMenu.SetBinding(FrameworkElement.VisibilityProperty, new Binding(AnnotationsControl.IsInEditModeProperty.Name) { Source = GetAnnotationsLayer(d), Converter = new BoolToVisibilityConverter() });
                GetMenuLayer(d).GetItemsSource<UIElement>().Add(controlMenu);

                layers.Add(GetAnnotationsLayer(d));
                layers.Add(GetActionsLayer(d));
                layers.Add(GetMenuLayer(d));

                SetContent(d, layers);

                SetAnnotationsChangedDelegate(d, (sender, changedEventArgs) =>
                {
                    switch (changedEventArgs.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            changedEventArgs.NewItems.OfType<AnnotationBase>().ToList().ForEach(_ => GetActionsLayer(d).GetItemsSource<ActionsAnnotationAdorner>().Add(_.ActionsAdorner));
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            changedEventArgs.OldItems.OfType<AnnotationBase>().ToList().ForEach(_ => GetActionsLayer(d).GetItemsSource<ActionsAnnotationAdorner>().Remove(_.ActionsAdorner));
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            GetActionsLayer(d).GetItemsSource<ActionsAnnotationAdorner>().Clear();
                            break;
                        default:
                            break;
                    }
                });
                GetAnnotationsLayer(d).Annotations.CollectionChanged += GetAnnotationsChangedDelegate(d);
            }
            SetIsVisible(d, (bool)e.NewValue);
        }

        public static void DestroyAnnotations(DependencyObject d)
        {
            var content = GetContent(d) as AdornmentCollection;
            var layer = AdornerLayer.GetAdornerLayer((Visual)d);
            var adorners = layer?.GetAdorners((UIElement)d);
            if (adorners == null) return;
            SetIsInEditMode(d, false);
            adorners?.ToList().ForEach(_ => layer.Remove(_));
            GetAnnotationsLayer(d).Annotations.CollectionChanged -= GetAnnotationsChangedDelegate(d);
            GetAnnotationsLayer(d).CleanUp();
            SetAnnotationsLayer(d, null);
            SetActionsLayer(d, null);
            SetMenuLayer(d, null);
            SetAnnotationsIsVisible(d, false);
            SetContent(d, null);
        }
    }
}
