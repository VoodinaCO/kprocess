using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Ext.Kprocess.Converters
{
    public class RadioButtonConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (int)value == (int)parameter;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
