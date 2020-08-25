using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class TaskbarHeightConverter : MarkupExtension, IValueConverter
    {
        public static double MinHeight = 728;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var height = MinHeight;
            if (parameter != null)
                double.TryParse((string)parameter, out height);

            var taskbarHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            var availableHeight = System.Windows.SystemParameters.WorkArea.Height - taskbarHeight;

            return Math.Min(height, availableHeight);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    public class TaskbarWidthConverter : MarkupExtension, IValueConverter
    {
        public static double MinWidth = 1024;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var width = MinWidth;
            if (parameter != null)
                double.TryParse((string)parameter, out width);

            var taskbarWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            var availableWidth = System.Windows.SystemParameters.WorkArea.Width - taskbarWidth;

            return Math.Min(width, availableWidth);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
