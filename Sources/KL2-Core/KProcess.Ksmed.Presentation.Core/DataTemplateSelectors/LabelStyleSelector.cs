using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
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
