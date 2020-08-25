using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class IsOkColorConverter : MarkupExtension, IValueConverter
    {
        public static readonly SolidColorBrush IsOkBrush = Brushes.Green;
        public static readonly SolidColorBrush IsNotOkBrush = Brushes.Orange;
        public static readonly SolidColorBrush KeyTaskIsNotOkBrush = Brushes.Red;
        public static readonly SolidColorBrush DefaultBrush = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PublishedAction publishedAction && publishedAction.IsOk == true)
                return IsOkBrush;
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    public class IsNotOkColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PublishedAction publishedAction && publishedAction.IsOk == false)
                return publishedAction.IsKeyTask ? IsOkColorConverter.KeyTaskIsNotOkBrush : IsOkColorConverter.IsNotOkBrush;
            return IsOkColorConverter.DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
