using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublishedResourceDataGridTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PublishedAction pAction && pAction.PublishedResource != null)
            {
                if (pAction.PublishedResource.FileHash != null && ExtensionsUtil.IsImageExtension(pAction.PublishedResource.File.Extension))
                    return Application.Current.Resources["ResourceImageTemplate"] as DataTemplate;
                if (pAction.PublishedResource.FileHash != null)
                    return Application.Current.Resources["ResourceUriTemplate"] as DataTemplate;
                return Application.Current.Resources["ResourceTextTemplate"] as DataTemplate;
            }
            return Application.Current.Resources["ResourceNullTemplate"] as DataTemplate;
        }
    }
}
