using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Renvoie True si Null
    /// </summary>
    public class CustomLabelPositionConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double playerHeight = (double)values[0];
                double playerWidth = (double)values[1];
                int videoHeight = (int)values[2];
                int videoWidth = (int)values[3];
                if (playerHeight == 0 || playerWidth == 0 || videoHeight == 0 ||videoWidth == 0) 
                    return Binding.DoNothing;
                if (videoHeight >= videoWidth)
                    return new Thickness(0);
                double ratio = playerWidth / videoWidth;
                return new Thickness(0, (playerHeight - videoHeight * ratio) / 2, 0, 0);
            }
            catch { return Binding.DoNothing; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
