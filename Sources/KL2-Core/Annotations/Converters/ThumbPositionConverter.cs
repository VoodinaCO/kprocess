using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AnnotationsLib.Converters
{
    public class ThumbPositionConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var handleSize = (double)values[0];

                var layoutTransform = (MatrixTransform)values[1];
                var scaledHandleSize = (layoutTransform.Transform(new Point(handleSize, handleSize))).X;

                var myValue = (parameter == null) ? (double)values[2] : double.Parse((string)parameter);
                return myValue - (scaledHandleSize / 2.0);
            }
            catch { return double.NaN; }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
