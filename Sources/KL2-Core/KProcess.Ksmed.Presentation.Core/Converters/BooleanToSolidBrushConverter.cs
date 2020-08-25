using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class BooleanToSolidBrushConverter : MarkupExtension, IValueConverter
    {
        static readonly SolidColorBrush TrueBrush = Brushes.Green;
        static readonly SolidColorBrush FalseBrush = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool val)
                return val ? TrueBrush : FalseBrush;
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
