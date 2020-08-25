using Kprocess.KL2.FileTransfer;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class HashToDownloadedFileConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hash = (string)value;
            if (string.IsNullOrEmpty(hash))
                return null;
            DirectoryInfo filesDirectory = new DirectoryInfo(Preferences.SyncDirectory);
            FileInfo[] filesInfo = filesDirectory.GetFiles($"{hash}*", SearchOption.TopDirectoryOnly);
            if (filesInfo == null || filesInfo.Length == 0)
                return null;
            string ext = filesInfo.First().Extension;
            Uri uri = new Uri(Path.Combine(Preferences.SyncDirectory, $"{hash}{ext}"), UriKind.Absolute);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            //image.Freeze();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
