using Kprocess.KL2.FileTransfer;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.FileTransferTest
{
    public class ProgressConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BackgroundTransferProgress bitsProgress)
                return $"({bitsProgress.FilesTransferred}/{bitsProgress.FilesTotal}) - {Math.Round(bitsProgress.PercentBytesTransferred)}%";
            if (value is TusTransferProgress tusProgress)
                return $"({tusProgress.FilesTransferred}/{tusProgress.FilesTotal}) - {Math.Round(tusProgress.PercentBytesTransferred)}%";
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
