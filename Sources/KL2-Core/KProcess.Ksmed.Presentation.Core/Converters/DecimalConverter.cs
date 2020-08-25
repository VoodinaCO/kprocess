using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class DecimalConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value?.ToString()
                .Replace(".", culture.NumberFormat.NumberDecimalSeparator)
                .Replace(",", culture.NumberFormat.NumberDecimalSeparator);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
                return null;
            return ValidateValueHelper.TryParse(value.ToString(), culture, out decimal? numValue) ? value : new ValidationResult(false, value);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}