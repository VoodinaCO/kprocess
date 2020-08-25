using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class UriToBitmapImageConverter : MarkupExtension, IValueConverter
    {
        static UriToBitmapImageConverter _instance;
        public static UriToBitmapImageConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UriToBitmapImageConverter();
                return _instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string uri = (string)value;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(uri, UriKind.Relative);
                image.EndInit();
                return image;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception : {e.Message}");
                if (e.InnerException != null)
                    Console.WriteLine($"Inner Exception : {e.InnerException.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
