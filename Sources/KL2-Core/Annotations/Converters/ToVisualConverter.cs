using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AnnotationsLib.Converters
{
    public class ToVisualBrushConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var visual = (FrameworkElement)values[0];
                var x = (double)values[1];
                var y = (double)values[2];
                var width = (double)values[3];
                var height = (double)values[4];
                var containerWidth = (double)values[5];
                var containerHeight = (double)values[6];
                var zoomFactor = (double)values[7];
                var visualBrush = new VisualBrush(visual);

                var zoomWidth = width / containerWidth / zoomFactor;
                var zoomHeight = height / containerHeight / zoomFactor;
                visualBrush.Viewbox = new Rect(
                    (x + width/2)/containerWidth - zoomWidth/2,
                    (y + height/2)/containerHeight - zoomHeight/2,
                    zoomWidth, zoomHeight);
                visualBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
                return visualBrush;
            }
            catch { return null; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
