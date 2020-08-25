using KProcess.Ksmed.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class IsQualifiedColorConverter : MarkupExtension, IValueConverter
    {
        public static readonly SolidColorBrush IsQualifiedBrush = Brushes.Green;
        public static readonly SolidColorBrush IsNotQualifiedBrush = Brushes.Orange;
        public static readonly SolidColorBrush KeyTaskIsNotQualifiedBrush = Brushes.Red;
        public static readonly SolidColorBrush DefaultBrush = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PublishedAction publishedAction && publishedAction.IsQualified == true)
                return IsQualifiedBrush;
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class IsNotQualifiedColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PublishedAction publishedAction && publishedAction.IsQualified == false)
                return publishedAction.IsKeyTask ? IsQualifiedColorConverter.KeyTaskIsNotQualifiedBrush : IsQualifiedColorConverter.IsNotQualifiedBrush;
            return IsQualifiedColorConverter.DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
