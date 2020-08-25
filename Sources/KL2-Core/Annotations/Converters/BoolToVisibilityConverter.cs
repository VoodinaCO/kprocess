using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib.Converters
{
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                var val = (bool)value;
                return val ? Visibility.Visible : Visibility.Hidden;
            }
            catch { return Visibility.Hidden; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val = (Visibility)value;
                return val == Visibility.Visible ? true : false;
            }
            catch { return false; }
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
