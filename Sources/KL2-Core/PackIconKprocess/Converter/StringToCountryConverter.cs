using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.PackIconKprocess
{
    public class StringToCountryConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string countryName && Enum.TryParse(countryName, out PackIconCountriesFlagsKind country))
                return country;
            return PackIconCountriesFlagsKind.France;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
