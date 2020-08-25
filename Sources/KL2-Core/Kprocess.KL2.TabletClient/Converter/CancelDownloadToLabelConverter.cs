using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Kprocess.KL2.TabletClient.ViewModel;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class CancelDownloadToLabelConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] is bool isConnected && values[1] is ITempPublication iTempPublication)
            {
                var cancellingAction = GetCancellingAction(isConnected, iTempPublication.PublicationIsNull);
                switch (cancellingAction)
                {
                    case CancellingAction.CancelDownload:
                        return Locator.LocalizationManager.GetValue("Transfer_Cancel");
                    case CancellingAction.ContinueOffline:
                        return Locator.LocalizationManager.GetValue("Transfer_ContinueOffline");
                    case CancellingAction.ExitApplication:
                        return Locator.LocalizationManager.GetValue("Transfer_ExitApplication");
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public static CancellingAction GetCancellingAction(bool isConnected, bool tempPublicationIsNull)
        {
            if (!isConnected && tempPublicationIsNull)
                return CancellingAction.ExitApplication;
            if (!isConnected && !tempPublicationIsNull)
                return CancellingAction.ContinueOffline;
            return CancellingAction.CancelDownload;
        }
    }

    public enum CancellingAction
    {
        CancelDownload,
        ContinueOffline,
        ExitApplication
    }
}
