using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublicationsTreeviewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element)
            {
                if (item is ProjectDir)
                    return element.FindResource("FolderTemplate") as DataTemplate;
                if (item is Procedure)
                    return element.FindResource("PublicationTemplate") as DataTemplate;
            }
            return null;
        }
    }
}
