using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class ThumbnailTooltipHeaderConverter : MarkupExtension, IMultiValueConverter
    {
        private StringFormatConverter stringConverter = new StringFormatConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
            stringConverter.Convert(values, targetType, values?[0] == null ? "{1}" : "{0} - {1}", culture);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
