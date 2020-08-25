using KProcess.Ksmed.Models;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PublishModeToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PublishMode mode && parameter is PublishModeEnum modeEnum)
                return mode.Id == modeEnum ? Visibility.Visible : Visibility.Hidden;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
