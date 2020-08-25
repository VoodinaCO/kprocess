using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Converts between TimeSpans and double-precision Seconds time measures
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class TimeSpanToSecondsConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
                return timeSpan.Ticks;
            if (value is Duration duration)
                return duration.HasTimeSpan ? duration.TimeSpan.Ticks : 0d;

            return 0d;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var test = value.GetType();
            if (value is long ticks_long)
            {
                if (targetType == typeof(TimeSpan))
                    return TimeSpan.FromTicks(ticks_long);
                if (targetType == typeof(Duration))
                    return new Duration(TimeSpan.FromTicks(ticks_long));
            }
            else if (value is double ticks_double)
            {
                if (targetType == typeof(TimeSpan))
                    return TimeSpan.FromTicks(System.Convert.ToInt64(ticks_double));
                if (targetType == typeof(Duration))
                    return new Duration(TimeSpan.FromTicks(System.Convert.ToInt64(ticks_double)));
            }

            return Activator.CreateInstance(targetType);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
