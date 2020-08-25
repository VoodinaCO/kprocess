using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PublishedRefItemTemplateSelector : DataTemplateSelector
    {
        static readonly List<string> ImageExtensions = new List<string> { ".png", ".jpg" };

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
                    if (refAction.PublishedReferential.FileHash != null && ImageExtensions.Contains(refAction.PublishedReferential.File.Extension))
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
