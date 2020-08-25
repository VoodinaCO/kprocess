using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Renvoie si le bouton Maximize/Restore thumbnail doit être caché
    /// </summary>
    public class ThumbnailMaximizeVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string thumbnailHash = (string)values[0];
                string videoHash = (string)values[1];
                if (string.IsNullOrEmpty(videoHash) && !string.IsNullOrEmpty(thumbnailHash))
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
            catch
            {
                return Visibility.Hidden;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
