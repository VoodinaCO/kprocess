using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib.Converters
{
    public class DoubleNegativeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val = (double)value;
                var param = parameter == null ? 0.0d : double.Parse((string)parameter);
                return -(val + param);
            }
            catch { return double.NaN; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var val = (double)value;
                var param = parameter == null ? 0.0d : double.Parse((string)parameter);
                return -(val + param);
            }
            catch { return double.NaN; }
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
