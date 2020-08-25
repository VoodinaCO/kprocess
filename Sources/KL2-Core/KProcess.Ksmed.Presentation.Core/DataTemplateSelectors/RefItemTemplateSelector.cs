using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class RefItemTemplateSelector : DataTemplateSelector
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".png", ".jpg" };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IReferentialActionLink refAction)
            {
                if (refAction.Referential.CloudFile != null)
                {
                    if (ImageExtensions.Contains(refAction.Referential.CloudFile.Extension))
                        return Application.Current.Resources["ImageTemplate"] as DataTemplate;
                    return Application.Current.Resources["UriTemplate"] as DataTemplate;
                }
                return Application.Current.Resources["TextTemplate"] as DataTemplate;
            }
            return null;
        }
    }

    public class RefItemLocalTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IReferentialActionLink refAction)
            {
                if (refAction.Referential.CloudFile != null)
                {
                    if (RefItemTemplateSelector.ImageExtensions.Contains(refAction.Referential.CloudFile.Extension))
                        return Application.Current.Resources["ImageLocalTemplate"] as DataTemplate;
                    return Application.Current.Resources["UriLocalTemplate"] as DataTemplate;
                }
                return Application.Current.Resources["TextLocalTemplate"] as DataTemplate;
            }
            return null;
        }
    }
}
