using AnnotationsLib.Annotations;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib.Converters
{
    public class ActionMenuPositionConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var annotation = (AnnotationBase)values[0];
                return annotation.ActionMenuPositionConverter.Convert(values, targetType, parameter, culture);
            }
            catch { throw new InvalidOperationException(); }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
