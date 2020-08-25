using Kprocess.KL2.FileTransfer;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class HashToUriConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hash = (string)value;
            if (string.IsNullOrEmpty(hash))
                return new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Resources\no-video.jpg"), UriKind.Absolute);
            DirectoryInfo filesDirectory = new DirectoryInfo(Preferences.SyncDirectory);
            FileInfo[] filesInfo = filesDirectory.GetFiles($"{hash}*", SearchOption.TopDirectoryOnly);
            if (filesInfo == null || filesInfo.Length == 0)
                return new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Resources\no-video.jpg"), UriKind.Absolute);
            string ext = filesInfo.First().Extension;
            Uri uri = new Uri(Path.Combine(Preferences.SyncDirectory, $"{hash}{ext}"), UriKind.Absolute);
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
