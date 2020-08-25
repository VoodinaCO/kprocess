namespace DlhSoft.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class DateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            DateTime time = (DateTime) value;
            string format = (parameter != null) ? ((string) parameter) : "g";
            return time.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime time;
            string str = (string) value;
            if (!string.IsNullOrEmpty(str) && DateTime.TryParse(str.Trim(), out time))
            {
                return time;
            }
            return null;
        }
    }
}

