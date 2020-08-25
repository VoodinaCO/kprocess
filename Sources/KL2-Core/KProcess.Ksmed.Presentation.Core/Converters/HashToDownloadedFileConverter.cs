using KProcess.KL2.APIClient;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class HashToDownloadedFileConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hash = (string)value;
            if (string.IsNullOrEmpty(hash))
                return null;
            DirectoryInfo filesDirectory = new DirectoryInfo(APIHttpClient.DownloadedFilesDirectory);
            FileInfo[] filesInfo = filesDirectory.GetFiles($"{hash}*", SearchOption.TopDirectoryOnly);
            if (filesInfo == null || filesInfo.Length == 0)
                return null;
            string ext = filesInfo.First().Extension;
            Uri uri = new Uri($"{APIHttpClient.DownloadedFilesDirectory}{hash}{ext}", UriKind.Relative);
            BitmapImage image = new BitmapImage(uri);
            image.Freeze();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class HashToUriConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hash = (string)value;
            if (string.IsNullOrEmpty(hash))
                return Binding.DoNothing;
            DirectoryInfo filesDirectory = new DirectoryInfo(APIHttpClient.DownloadedFilesDirectory);
            FileInfo[] filesInfo = filesDirectory.GetFiles($"{hash}*", SearchOption.TopDirectoryOnly);
            if (filesInfo == null || filesInfo.Length == 0)
                return Binding.DoNothing;
            string ext = filesInfo.First().Extension;
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), APIHttpClient.DownloadedFilesDirectory, $"{hash}{ext}"), UriKind.Absolute);
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
