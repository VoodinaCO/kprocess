using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Ext.Kprocess.Converters
{
    public class AndOrComboBoxVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        private static ToggleAndOrButtonConverter _converter = new ToggleAndOrButtonConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
            (bool)_converter.Convert(values, targetType, parameter, culture) ? Visibility.Visible : Visibility.Collapsed;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
