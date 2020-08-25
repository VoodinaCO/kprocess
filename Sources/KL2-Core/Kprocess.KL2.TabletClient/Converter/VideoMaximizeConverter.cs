using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class VideoMaximizeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                bool videoIsMaximize = (bool)value;
                string param = (string)parameter;
                if (param == "Maximize" && !videoIsMaximize)
                    return Visibility.Visible;
                if (param == "Restore" && videoIsMaximize)
                    return Visibility.Visible;
                if (param == "Other" && !videoIsMaximize)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
