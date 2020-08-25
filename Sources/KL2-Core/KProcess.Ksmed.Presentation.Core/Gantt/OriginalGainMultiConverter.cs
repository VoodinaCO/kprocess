using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class OriginalGainMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var percentage = values[0] is double ? (double)values[0] : 0.0;
            var totalWidth = values[1] is double ? (double)values[1] : 0.0;

            return totalWidth * percentage / 100;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
