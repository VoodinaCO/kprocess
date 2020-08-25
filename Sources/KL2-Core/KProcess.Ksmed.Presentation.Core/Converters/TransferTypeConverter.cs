using Kprocess.KL2.FileTransfer;
using KProcess.Globalization;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class TransferTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITransferOperation transferOperation)
            {
                if (transferOperation.JobType == JobType.Download)
                    return LocalizationManager.GetString("View_PrepareVideos_Receiving");
                
                return LocalizationManager.GetString("View_PrepareVideos_Sending");
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
