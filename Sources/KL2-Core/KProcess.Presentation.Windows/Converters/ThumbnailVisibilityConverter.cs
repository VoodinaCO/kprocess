using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    public class ThumbnailVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] is bool showThumbnail && values[1] is Visibility selectorVisibility)
            {
                if (showThumbnail && selectorVisibility != Visibility.Visible)
                    return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
