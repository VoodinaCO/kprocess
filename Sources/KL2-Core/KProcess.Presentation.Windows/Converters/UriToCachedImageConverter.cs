using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace KProcess.Presentation.Windows.Converters
{
    public class UriToCachedImageConverter : MarkupExtension, IValueConverter
    {
        static UriToCachedImageConverter _instance;
        public static UriToCachedImageConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UriToCachedImageConverter();
                return _instance;
            }
        }

        public BitmapImage Convert(Uri uri) =>
            (BitmapImage)Convert(uri, typeof(BitmapImage), null, null);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Uri uri))
                return null;

            if (!File.Exists(uri.LocalPath))
                return null;

            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = uri;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class CloudFileToCachedImageConverter : MarkupExtension, IValueConverter
    {
        static CloudFileToCachedImageConverter _instance;
        public static CloudFileToCachedImageConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CloudFileToCachedImageConverter();
                return _instance;
            }
        }

        public BitmapImage Convert(Uri uri) =>
            (BitmapImage)Convert(uri, typeof(BitmapImage), null, null);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CloudFile cloudFile))
                return null;

            Uri uri;
            if (File.Exists(cloudFile.LocalUri.LocalPath))
                uri = cloudFile.LocalUri;
            else
                uri = cloudFile.Uri;

            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = uri;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class CloudFileToUriConverter : MarkupExtension, IValueConverter
    {
        static CloudFileToUriConverter _instance;
        public static CloudFileToUriConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CloudFileToUriConverter();
                return _instance;
            }
        }

        public BitmapImage Convert(Uri uri) =>
            (BitmapImage)Convert(uri, typeof(BitmapImage), null, null);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CloudFile cloudFile))
                return null;

            Uri uri;
            if (File.Exists(cloudFile.LocalUri.LocalPath))
                uri = cloudFile.LocalUri;
            else
                uri = cloudFile.Uri;

            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
