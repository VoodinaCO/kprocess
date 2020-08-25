using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib.Converters
{
    public class ScaleConverter : MarkupExtension, IMultiValueConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var param = parameter == null ? 1.0d : double.Parse((string)parameter);
                var first = (double)value;
                return first * param;
            }
            catch { return double.NaN; }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (double)values[0] * (double)values[1];
            }
            catch { return double.NaN; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter == null ? 1.0d : double.Parse((string)parameter);
            var first = (double)value;
            return first / param;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
