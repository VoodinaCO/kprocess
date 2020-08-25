using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib.Converters
{
    public class SubstractConverter : MarkupExtension, IMultiValueConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var param = parameter == null ? 0.0d : double.Parse((string)parameter);
                var first = (double)value;
                return first - param;
            }
            catch { return double.NaN; }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var param = parameter == null ? 0.0d : double.Parse((string)parameter);
                var first = (double)values[0];
                return 2 * first - values.Cast<double>().Sum() - param;
            }
            catch { return double.NaN; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
