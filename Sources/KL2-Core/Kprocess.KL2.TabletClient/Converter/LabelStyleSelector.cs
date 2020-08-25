using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class LabelStyleSelectorWithTriggers : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PublishedAction publishedAction)
            {
                if (publishedAction.IsGroup)
                    return Application.Current.Resources["LabelGroupStyleSelectorWithTriggers"] as DataTemplate;
                if (publishedAction.LinkedPublication != null)
                    return Application.Current.Resources["LabelLinkedPublishedActionStyleSelectorWithTriggers"] as DataTemplate;
                return Application.Current.Resources["LabelPublishedActionStyleSelectorWithTriggers"] as DataTemplate;
            }
            return null;
        }
    }

    public class LabelStyleSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PublishedAction publishedAction)
            {
                if (publishedAction.IsGroup)
                    return Application.Current.Resources["LabelGroupStyleSelector"] as DataTemplate;
                if (publishedAction.LinkedPublication != null)
                    return Application.Current.Resources["LabelLinkedPublishedActionStyleSelector"] as DataTemplate;
                return Application.Current.Resources["LabelPublishedActionStyleSelector"] as DataTemplate;
            }
            return null;
        }
    }
}
