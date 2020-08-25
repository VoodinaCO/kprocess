using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Unosquare.FFME.Common;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Renvoie si le thumbnail doit être caché
    /// </summary>
    public class ThumbnailVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values[0] is string thumbnailHash && values[2] is MediaPlaybackState playerMediaState && values[3] is TimeSpan playerPosition)
            {
                string videoHash = values[1] as string;
                if (string.IsNullOrEmpty(videoHash) && !string.IsNullOrEmpty(thumbnailHash))
                    return Visibility.Visible;
                if (!string.IsNullOrEmpty(thumbnailHash) && (playerMediaState == MediaPlaybackState.Close || playerMediaState == MediaPlaybackState.Stop || (playerMediaState == MediaPlaybackState.Pause && playerPosition.TotalMilliseconds == 0)))
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
