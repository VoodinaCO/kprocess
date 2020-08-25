using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace KProcess.KL2.APIClient
{
    public class DownloadFileConverter : MarkupExtension, IValueConverter
    {
        readonly IAPIHttpClient _apiHttpClient;

        public DownloadFileConverter(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(string))
                return null;

            if (_apiHttpClient.DownloadedFiles.ContainsKey((string)value))
            {
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri(_apiHttpClient.DownloadedFiles[(string)value], UriKind.Absolute);
                    image.EndInit();
                    return image;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Une erreur s'est produite lors du téléchargement du fichier {(string)value}");
                    Console.WriteLine($"Exception : {e.Message}");
                    if (e.InnerException != null)
                        Console.WriteLine($"Inner Exception : {e.InnerException.Message}");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
