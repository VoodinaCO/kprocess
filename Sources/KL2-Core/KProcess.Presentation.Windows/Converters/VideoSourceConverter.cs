using Kprocess.KL2.FileTransfer;
using KProcess.Ksmed.Models;
using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Converter qui renvoit la source d'une video selon son état de synchronisation
    /// </summary>
    public class VideoSourceConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Video video)
            {
                if (video.Sync && video.IsSync)
                    return Path.Combine(Preferences.SyncDirectory, video.Filename);
                if (video.OnServer == true)
                    return $"{Preferences.FileServerUri}/Stream/{video.Filename}";
                if (File.Exists(video.FilePath))
                    return video.FilePath;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
