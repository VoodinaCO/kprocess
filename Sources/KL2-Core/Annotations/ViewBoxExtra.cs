using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AnnotationsLib
{
    public class ViewBoxExtra
    {
        public static readonly DependencyProperty DisableScalingProperty = DependencyProperty.RegisterAttached("DisableScaling", typeof(bool), typeof(Viewbox), new UIPropertyMetadata(OnDisableScalingPropertyChanged));
        public static readonly DependencyProperty NonScaledObjectsProperty = DependencyProperty.RegisterAttached("NonScaledObjects", typeof(List<DependencyObject>), typeof(Viewbox), null);

        public static bool GetDisableScaling(DependencyObject element)
        {
            return (bool)element.GetValue(DisableScalingProperty);
        }
        public static void SetDisableScaling(DependencyObject element, bool value)
        {
            element.SetValue(DisableScalingProperty, value);
        }

        public static List<DependencyObject> GetNonScaledObjectsProperty(DependencyObject element)
        {
            return (List<DependencyObject>)element.GetValue(NonScaledObjectsProperty);
        }
        public static void SetNonScaledObjectsProperty(DependencyObject element, List<DependencyObject> value)
        {
            element.SetValue(NonScaledObjectsProperty, value);
        }

        private static void OnDisableScalingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)d;
            if (element.IsLoaded) AddScalingHandler(element, null);
            else element.Loaded += AddScalingHandler;
        }

        private static void AddScalingHandler(object sender, EventArgs e)
        {
            try
            {
                var newValue = GetDisableScaling((DependencyObject)sender);
                var viewBox = ObtainParentViewbox((DependencyObject)sender);
                var nonScaleObjects = (List<DependencyObject>)viewBox.GetValue(NonScaledObjectsProperty);
                if (newValue && nonScaleObjects == null)
                {
                    nonScaleObjects = new List<DependencyObject>();
                    viewBox.SetValue(NonScaledObjectsProperty, nonScaleObjects);
                    viewBox.SizeChanged += ViewBox_SizeChanged;
                }

                if (newValue)
                {
                    nonScaleObjects.Add((DependencyObject)sender);
                }
                else
                {
                    nonScaleObjects.Remove((DependencyObject)sender);
                }
            }
            catch (NullReferenceException exc)
            {
                throw new Exception("The element must be contained inside an ViewBoxExtra", exc);
            }
        }

        private static void ViewBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var viewBox = sender as Viewbox;
            var transform = ((ContainerVisual)VisualTreeHelper.GetChild(sender as DependencyObject, 0)).Transform;
            if (transform != null && viewBox != null)
            {
                foreach (var nonScaleObject in (List<DependencyObject>)viewBox.GetValue(NonScaledObjectsProperty))
                {
                    var element = (FrameworkElement)nonScaleObject;
                    element.LayoutTransform = (Transform)transform.Inverse;
                }
            }
        }

        private static Viewbox ObtainParentViewbox(DependencyObject d)
        {
            var parent = VisualTreeHelper.GetParent(d);
            return parent is Viewbox ? parent as Viewbox : ObtainParentViewbox(parent);
        }
    }
}
