using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertit des ticks en DateTime.
    /// </summary>
    /// <remarks>A n'utiliser que dans le cadre du Gantt.</remarks>
    public class GanttTicksToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ticks = System.Convert.ToInt64(value);
            return GanttDates.ToDateTime(ticks);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = System.Convert.ToDateTime(value);
            return GanttDates.ToTicks(dateTime);
        }
    }
}
