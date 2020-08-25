using AnnotationsLib.Annotations.Actions;
using System.Windows;
using System.Windows.Controls;

namespace AnnotationsLib.Annotations
{
    public class ActionsAnnotationDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item.GetType() == typeof(ChangeBrushAction))
                return element.FindResource("ChangeBrushItemTemplate") as DataTemplate;
            else
                return element.FindResource("DefaultItemTemplate") as DataTemplate;
        }

        public static ActionsAnnotationDataTemplateSelector Instance { get; } = new ActionsAnnotationDataTemplateSelector();
    }
}
