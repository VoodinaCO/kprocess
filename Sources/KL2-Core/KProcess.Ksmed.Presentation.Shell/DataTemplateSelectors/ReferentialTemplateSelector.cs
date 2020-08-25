using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    public class ReferentialTemplateSelector : DataTemplateSelector
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".png", ".jpg" };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DependencyObject depObj = container;
            while (depObj.GetType() != typeof(AdminReferentialsView))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
                if (depObj == null)
                    return null;
            }
            if (depObj is FrameworkElement element)
            {
                if (item is IActionReferential refAction)
                {
                    if (refAction.CloudFile != null)
                    {
                        if (ImageExtensions.Contains(refAction.CloudFile.Extension))
                            return element.FindResource("ReferentialImageTemplate") as DataTemplate;
                        return element.FindResource("ReferentialUriTemplate") as DataTemplate;
                    }
                    return element.FindResource("ReferentialEmptyTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }

    public class ReferentialHasImageConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IActionReferential refAction && refAction.CloudFile != null)
                return ReferentialTemplateSelector.ImageExtensions.Contains(refAction.CloudFile.Extension);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class ReferentialHasUriConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IActionReferential refAction && refAction.CloudFile != null)
                return !ReferentialTemplateSelector.ImageExtensions.Contains(refAction.CloudFile.Extension);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
