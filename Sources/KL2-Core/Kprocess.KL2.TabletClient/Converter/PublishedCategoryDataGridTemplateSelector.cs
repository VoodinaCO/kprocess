using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublishedCategoryDataGridTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PublishedAction pAction && pAction.PublishedActionCategory != null)
            {
                if (pAction.PublishedActionCategory.FileHash != null && ExtensionsUtil.IsImageExtension(pAction.PublishedActionCategory.File.Extension))
                    return Application.Current.Resources["CategoryImageTemplate"] as DataTemplate;
                if (pAction.PublishedActionCategory.FileHash != null)
                    return Application.Current.Resources["CategoryUriTemplate"] as DataTemplate;
                return Application.Current.Resources["CategoryTextTemplate"] as DataTemplate;
            }
            return Application.Current.Resources["CategoryNullTemplate"] as DataTemplate;
        }
    }
}
