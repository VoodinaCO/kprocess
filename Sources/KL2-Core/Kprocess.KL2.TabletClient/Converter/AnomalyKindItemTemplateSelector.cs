using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class AnomalyKindItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element)
            {
                if (item is AnomalyKindItemTitle)
                    return element.FindResource("AnomalyKindItemTitleTemplate") as DataTemplate;
                if (item is AnomalyKindItem)
                    return element.FindResource("AnomalyKindItemTemplate") as DataTemplate;
                /*if (item is AnomalyKindEditableItem)
                    return element.FindResource("AnomalyKindEditableItemTemplate") as DataTemplate;*/
                if (item is AnomalyKindItemEmpty)
                    return element.FindResource("AnomalyKindItemEmptyTemplate") as DataTemplate;
                return null;
            }
            return null;
        }
    }
}
