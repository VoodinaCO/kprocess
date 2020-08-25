using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublishedRefItemDataGridTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DependencyObject depObj = container;
            while (depObj.GetType() != typeof(SfDataGrid))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
                if (depObj == null)
                    return null;
            }
            if (depObj is FrameworkElement element)
            {
                if (item is PublishedReferentialAction refAction)
                {
                    if (refAction.PublishedReferential.FileHash != null && ExtensionsUtil.IsImageExtension(refAction.PublishedReferential.File.Extension))
                        return element.FindResource("ImageTemplate") as DataTemplate;
                    if (refAction.PublishedReferential.FileHash != null)
                        return element.FindResource("UriTemplate") as DataTemplate;
                    return element.FindResource("TextTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }
}
