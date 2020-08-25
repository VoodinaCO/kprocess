using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertit des valeurs en visibilité.
    /// </summary>
    public class OriginalGainVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var chartVisibility = values[0] is Visibility ? (Visibility)values[0] : Visibility.Collapsed;
            var gainValue = values[1] is double? ? (double?)values[1] : null;

            return chartVisibility == Visibility.Visible && gainValue.HasValue ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
